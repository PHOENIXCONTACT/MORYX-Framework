// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Logging;
using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// - There is only a single slot for the running job
/// - There are unlimited slots for completing jobs
/// - The job from the running slot is moved to completing when it starts the clean-up **or** a job is completing and a new job could be started
/// - Slots of completing jobs are created on-demand
/// - Slots of completing jobs are destroyed upon completion
/// - Slots are not transferred between jobs and recipes
/// - Slots of completing jobs do not accept new running jobs. Those will remain Idle until the running slot is available
/// </summary>
[ExpectedConfig(typeof(SeamlessSchedulerConfig))]
[Plugin(LifeCycle.Singleton, typeof(IJobScheduler), Name = nameof(SeamlessScheduler))]
internal sealed class SeamlessScheduler : JobSchedulerBase<SeamlessSchedulerConfig>, ILoggingComponent
{
    private JobSlotWrapper _running;
    private JobSlots _completingSlots;
    private JobSlots _resumeSlots;

    public IModuleLogger Logger { get; set; }

    /// <inheritdoc />
    public override void Initialize(JobSchedulerConfig config)
    {
        base.Initialize(config);

        _running = new JobSlotWrapper();
        _completingSlots = new JobSlots(0);
        _resumeSlots = new JobSlots(0);
    }

    public override IEnumerable<Job> SchedulableJobs(IEnumerable<Job> jobs)
    {
        var allJobs = jobs.ToArray();
        // Option 1: No jobs to be scheduled in the first place
        if (allJobs.Length == 0)
        {
            yield break;
        }
        // Option 2: System restarted and we resume jobs
        else if (allJobs.Any(HadStarted))
        {
            foreach (var job in allJobs.Where(HadStarted))
            {
                yield return job;
            }
        }
        // Option 3:System idle or only occupied by completing jobs with no follow up
        else if (_running.Target == null
            || _running.Target.Classification == JobClassification.Completing && JobList.Next(_running.Target).IsCleanup())
        {
            Job last = null;
            // First job and any follow ups are schedulable and take the slot
            foreach (var job in allJobs)
            {
                // Initial job or follow up
                if (last == null || last.SameRecipeAs(job))
                    yield return job;
                else
                    yield break;
                last = job;
            }
        }
        // Option 4: Running slot occupied and still required
        else
        {
            // We only accept follow-ups of the running job or its follow-ups
            foreach (var job in allJobs)
            {
                var previous = JobList.Previous(job);
                if (_running.Target.SameRecipeAs(job) && job.SameRecipeAs(previous))
                {
                    yield return job;
                }
            }
        }
    }

    /// <summary>
    /// Jobs are reloaded without completed processes and might have been running without currently running processes at shut down
    /// </summary>
    private static bool HadStarted(Job j) => j.RunningProcesses.Count > 0 || j.SuccessCount > 0 || j.FailureCount > 0;

    public override void JobsReady(IEnumerable<Job> startableJobs)
    {
        var runningJob = _running.Target;
        // If the running slot is currently assigned to a running job, leave these waiting
        if (runningJob?.Classification <= JobClassification.Running)
        {
            return;
        }

        // If the RunningSlot is currently assigned, with a completing job, we move it
        if (runningJob is not null &&
            (IsCompletingProductionFollowedByCleanUp(runningJob) || IsCompletingSetupNotFollowedByProduction(runningJob)))
        {
            // Move to completing slots
            MoveToCompleting(runningJob, JobSource.FromRunning);
        }

        var allJobs = startableJobs.ToList();
        var first = allJobs[0];

        // If the running slot falls back to a completing group, remove it from completing
        var previous = JobList.Previous(first);
        if (first.SameRecipeAs(previous) && previous.Classification == JobClassification.Completing)
        {
            TryReleaseCompleting(previous);
        }

        // Iterate all jobs and assign to running or resume slots
        Job last = null;
        foreach (var job in allJobs)
        {
            if (job == first && job.RunningProcesses.Count == 0)
            {
                AssignRunning(job);
            }
            else if (HadStarted(job))
            {
                // Previously running jobs are resumed
                if (job.SameRecipeAs(last))
                {
                    // Follow-up to already resumed job with slot
                    ReplaceResumed(last, job);
                }
                else
                {
                    // Provide new resume slot
                    AssignResumed(job);
                }
            }
            last = job;
        }
    }

    // WARNING: We assume production jobs following on a setup to be waiting already,
    // as there is no direct reference between setup and its first production job
    private bool IsCompletingSetupNotFollowedByProduction(Job target)
        => target.Classification == JobClassification.Completing && target.IsPreparingSetup() && JobList.Next(target).Classification != JobClassification.Waiting;

    private bool IsCompletingProductionFollowedByCleanUp(Job target)
        => target.Classification == JobClassification.Completing && JobList.Next(target).IsCleanup();

    public override void JobUpdated(Job job, JobClassification classification)
    {
        // After restart we move jobs from resume slots to productive slots
        if (_resumeSlots.HasSlot(job))
        {
            if (classification == JobClassification.Running)
            {
                MoveToRunning(job, JobSource.FromResumed);
            }
            else if (classification == JobClassification.Completing)
            {
                MoveToCompleting(job, JobSource.FromResumed);
            }

            // If we resumed everything, we can reassign running with new jobs
            if (_resumeSlots.Size == 0 && _running.Target == null)
            {
                RaiseSlotAvailable();
            }
            return;
        }

        // Changes below Completing do not interest us
        if (classification < JobClassification.Completing)
        {
            return;
        }

        // If the completed job was a cleanup or a setup which was moved to completing, remove the slot
        if (classification == JobClassification.Completed && (job.IsCleanup() || job.IsPrepare()) && TryReleaseCompleting(job))
        {
            return;
        }

        // Find the next waiting job
        var next = JobList.Forward(job).FirstOrDefault(n => n.Classification == JobClassification.Waiting);
        if (next is null)
        {
            // There is a completing setup, for which the production job is gone, e.g. aborted
            if (classification == JobClassification.Completing && job.IsPrepare())
            {
                MoveToCompleting(job, JobSource.FromRunning);
                RaiseSlotAvailable();
            }
            return;
        }

        if (_running.Target == job && next.SameRecipeAs(job))
        {
            // Next job is a follow-up for the completing job
            AssignRunning(next);
        }
        else if (_running.Target == job && classification == JobClassification.Completing && next.IsCleanupOf(job))
        {
            // We are completing the job and there is no follow-up, so we COULD reassign the running slot
            RaiseSlotAvailable();
        }
        else if (_running.Target == job && classification == JobClassification.Completed)
        {
            if (job.IsPrepareOf(next))
            {
                // If the completed job was a prepare, start its production target
                AssignRunning(next);
            }
            else if (next.IsCleanupOf(job) && next.IsCleanupOf(JobList.Previous(job)))
            {
                // Completing jobs completed in reverse order
                var previous = JobList.Previous(job);
                ReplaceRunning(job, previous);
            }
            else if (job.IsPrepare() && next.IsCleanup() || next.IsCleanupOf(job))
            {
                // A completing job on running slot has completed, start the cleanup
                MoveToCompleting(next, JobSource.FromRunning);
                RaiseJobScheduled(next);
                // Already raised if job is not a prepare, but doesn't hurt and necessary,
                // if production job was aborted between setup and clean-up
                RaiseSlotAvailable();
            }
        }
        else if (classification == JobClassification.Completed && next.IsCleanupOf(job))
        {
            var previous = JobList.Previous(job);
            if (next.IsCleanupOf(previous) && TryReplaceCompleting(job, previous))
            {
                // Completing jobs completed in reverse order
            }
            else if (TryReplaceCompleting(job, next))
            {
                // A completing job on completing slot has completed, start the cleanup
                RaiseJobScheduled(next);
            }
        }
    }

    /// <summary>
    /// Replaces the <paramref name="running"/> job with a <paramref name="previous"/> job
    /// </summary>
    private void ReplaceRunning(Job running, Job previous)
    {
        _running.Assign(previous);
        Logger.LogTrace("{classification} job {id} replaced {runningClassification} job {runningId} in running slot",
            previous.Classification, previous.Id, running.Classification, running.Id);
        // We might have raised the event already, when running switched to completing, but this switch is not guaranteed
        RaiseSlotAvailable();
    }

    /// <summary>
    /// Replaces the <paramref name="completing"/> job with a <paramref name="previous"/> job
    /// </summary>
    private bool TryReplaceCompleting(Job completing, Job previous)
    {
        if (!_completingSlots.TryReplace(completing, previous))
        {
            return false;
        }
        Logger.LogTrace("{classification} job {id} replaced {previousClassification} job {previousId} in completing slot",
            previous.Classification, previous.Id, completing.Classification, completing.Id);
        return true;
    }

    /// <summary>
    /// Assign the running slot to new <paramref name="job"/>
    /// </summary>
    private void AssignRunning(Job job)
    {
        _running.Assign(job);
        Logger.LogTrace("{classification} job {id} assigned to running slot", job.Classification, job.Id);
        RaiseJobScheduled(job);
    }

    /// <summary>
    /// Moved the <paramref name="job"/> from the <paramref name="source"/> to the running slot
    /// </summary>
    /// <param name="job"></param>
    /// <param name="source"></param>
    private void MoveToRunning(Job job, JobSource source)
    {
        if (source != JobSource.FromResumed)
        {
            return;
        }
        _resumeSlots.TryRelease(job);
        _resumeSlots.TryResize(_resumeSlots.Size - _resumeSlots.AvailableSlots);
        _running.Assign(job);
        Logger.LogTrace("{classification} job {id} moved from resume to running slot", job.Classification, job.Id);
    }

    /// <summary>
    /// Move <paramref name="job"/> completing slots, optionally clearing the running slot
    /// </summary>
    private void MoveToCompleting(Job job, JobSource source)
    {
        _completingSlots.TryResize(_completingSlots.Size + 1);
        if (!_completingSlots.TryAssign(job))
        {
            return;
        }

        if (source == JobSource.FromRunning)
        {
            _running.Assign(null);
            Logger.LogTrace("{classification} job {id} moved from running to completing slot", job.Classification, job.Id);
        }
        else if (source == JobSource.FromResumed && _resumeSlots.TryRelease(job))
        {
            _resumeSlots.TryResize(_resumeSlots.Size - _resumeSlots.AvailableSlots);
            Logger.LogTrace("{classification} job {id} moved from running to completing slot", job.Classification, job.Id);
        }
    }

    /// <summary>
    /// Try releasing completing slot occupied by the <paramref name="job"/>
    /// </summary>
    private bool TryReleaseCompleting(Job job)
    {
        if (!_completingSlots.TryRelease(job))
        {
            return false;
        }

        Logger.LogTrace("{classification} Job {id} released a completing slots", job.Classification, job.Id);
        _completingSlots.TryResize(_completingSlots.Size - 1);
        return true;
    }

    /// <summary>
    /// Assign a new resume slot to the given <paramref name="job"/>
    /// </summary>
    /// <param name="job"></param>
    private void AssignResumed(Job job)
    {
        _resumeSlots.TryResize(_resumeSlots.Size + 1);
        if (!_resumeSlots.TryAssign(job))
        {
            return;
        }

        Logger.LogTrace("{classification} job {id} assigned to resume slot", job.Classification, job.Id);
        RaiseJobScheduled(job);
    }

    /// <summary>
    /// Try replacing a resumed job in the resume slots with a follow-up <paramref name="job"/>
    /// </summary>
    private void ReplaceResumed(Job previous, Job job)
    {
        if (!_resumeSlots.TryReplace(previous, job))
        {
            return;
        }
        Logger.LogTrace("{classification} job {id} replaced {previousClassification} job {previousId} in completing slot",
            job.Classification, job.Id, previous.Classification, previous.Id);
        RaiseJobScheduled(job);
    }

    private enum JobSource
    {
        FromRunning,
        FromResumed,
        FromCompleting
    }
}

