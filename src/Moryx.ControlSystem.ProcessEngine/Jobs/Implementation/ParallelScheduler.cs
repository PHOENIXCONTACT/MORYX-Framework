// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// - Configured number of availableSlots
/// - Slots are assigned from Prepare to clean-up
/// - Jobs of the same recipe can be added to availableSlots
/// - Slots are only released, when all jobs of the slot are completed
/// - There can be more than one running job
/// - Slots are not transferred between different recipes. They are released and reassigned
/// </summary>
[ExpectedConfig(typeof(ParallelSchedulerConfig))]
[Plugin(LifeCycle.Singleton, typeof(IJobScheduler), Name = nameof(ParallelScheduler))]
internal class ParallelScheduler : JobSchedulerBase<ParallelSchedulerConfig>, ILoggingComponent
{
    private JobSlots _slots;

    public IModuleLogger Logger { get; set; }

    /// <inheritdoc />
    public override void Initialize(JobSchedulerConfig config)
    {
        base.Initialize(config);

        _slots = new JobSlots(Config.MaxActiveJobs);
    }

    /// <inheritdoc/>
    public override IEnumerable<Job> SchedulableJobs(IEnumerable<Job> jobs)
    {
        // Create collection to work on
        var schedulableJobs = jobs.ToList();

        Job last = null;
        var availableSlots = _slots.AvailableSlots;
        for (var index = 0; index < schedulableJobs.Count;)
        {
            var job = schedulableJobs[index];
            var previous = JobList.Previous(job);
            var currentSlot = _slots.FirstOrDefault(s => s.Target.SameRecipeAs(job));
            // This recipe currently holds a slot and the job was placed as a follow-up
            if (currentSlot != null && job.SameRecipeAs(previous)
                                    && previous.Classification >= JobClassification.Waiting)
            {
                // This is simply appended after the production job that holds the slot
                index++;
            }
            else if (job.SameRecipeAs(last) && schedulableJobs.Contains(last))
            {
                // If the last one was schedulable, this one is too
                index++;
            }
            else if (availableSlots > 0)
            {
                // Job is going to occupy a slot
                availableSlots--;
                index++;
            }
            else
            {
                // We can not schedule this atm
                schedulableJobs.Remove(job);
            }

            last = job;
        }

        return schedulableJobs;
    }

    /// <inheritdoc/>
    public override void JobsReady(IEnumerable<Job> startableJobs) => startableJobs.ForEach(JobReady);

    private void JobReady(Job job)
    {
        var last = JobList.Previous(job);

        // If the last job was a setup, declare dependency for production job
        if (last.IsPrepareOf(job))
        {
            AddDependency(job, last);
        }

        // If this is a cleanup, declare dependency on previous job
        if (job.IsCleanupOf(last))
        {
            AddDependency(job, last);
        }

        var followUp = job.SameRecipeAs(last);
        // This is a follow-up job for running job with the same recipe
        if (followUp && last.Classification < JobClassification.Completing)
        {
            AddDependency(job, last);
        }

        // If the job has dependencies, we cannot process it right now
        if (Dependencies(job).Any())
        {
            return;
        }

        if (followUp)
        {
            ScheduleIntoSlotOf(last, job);
        }
        else
        {
            ScheduleIntoSlot(job);
        }
    }

    /// <summary>
    /// Replaces the <paramref name="current"/> job in a slot with the <paramref name="replacement"/> job
    /// </summary>
    private void ScheduleIntoSlotOf(Job current, Job replacement)
    {
        if (!_slots.TryReplace(current, replacement))
        {
            return;
        }
        Logger.LogTrace("{classification} job {id} replaced {previousClassification} job {previousId} in scheduled slot",
            replacement.Classification, replacement.Id, current.Classification, current.Id);

        RaiseJobScheduled(replacement);
    }

    /// <summary>
    /// Assign a slot to a new <paramref name="job"/>
    /// </summary>
    private void ScheduleIntoSlot(Job job)
    {
        if (!_slots.TryAssign(job))
        {
            return;
        }
        Logger.LogTrace("{classification} job {id} assigned to running slot", job.Classification, job.Id);
        RaiseJobScheduled(job);
    }

    /// <inheritdoc/>
    public override void JobUpdated(Job job, JobClassification classification)
    {
        // Changes below Completing do not interest us
        if (classification < JobClassification.Completing)
        {
            return;
        }

        // If a cleanup was completed: Release the slot
        if (classification == JobClassification.Completed && job.IsCleanup())
        {
            ReleaseSlotOf(job);
            return;
        }

        var next = JobList.Next(job);
        // If a setup was completed, but the following jobs are already gone (e.g. aborted): Release the slot
        if (classification == JobClassification.Completed && job.IsPrepare() && next?.Classification != JobClassification.Waiting)
        {
            ReleaseSlotOf(job);
            return;
        }

        // If there is no next, waiting job: There is nothing to do
        if (next?.Classification != JobClassification.Waiting)
        {
            return;
        }

        // If there is a waiting follow-up for a completing job: Replace the job in the slot with the follow-up
        if (classification == JobClassification.Completing && next.SameRecipeAs(job) && RemoveDependency(next, job))
        {
            ScheduleIntoSlotOf(job, next);
        }

        // Only apply other opertions on availableSlots after job completion
        if (classification == JobClassification.Completing)
        {
            return;
        }

        // Remove possible dependency on next job
        RemoveDependency(next, job);

        // If the completed job was a prepare: Replace the job in the slot with the production job
        if (job.IsPrepareOf(next))
        {
            ScheduleIntoSlotOf(job, next);
        }
        // If the completed job was a prepare and the next is the corresponding clean-up: Replace the job in the slot with the clean-up
        else if (job.IsPrepare() && next.IsCleanup())
        {
            ScheduleIntoSlotOf(job, next);
        }
        // If the completed job was the follow-up of a not yet completed job: Replace the job in the slot with the still running production
        else if (next.IsCleanupOf(job) && next.IsCleanupOf(JobList.Previous(job)))
        {
            ReassignSlotToPrevious(job);
        }
        // If we completed the last production job and a clean-up is waiting: Replace the job in the slot with the clean up
        else if (next.IsCleanupOf(job))
        {
            ScheduleIntoSlotOf(job, next);
        }
    }

    /// <summary>
    /// Releasing the slot occupied by the given <paramref name="job"/>
    /// </summary>
    private void ReleaseSlotOf(Job job)
    {
        if (!_slots.TryRelease(job))
        {
            Logger.LogError("{classification} Job {id} failed to released its availableSlots", job.Classification, job.Id);
            return;
        }

        Logger.LogTrace("{classification} Job {id} released its availableSlots", job.Classification, job.Id);
        RaiseSlotAvailable();
    }

    /// <summary>
    /// Keeps the scheduled slot but moves it back to the previous job
    /// </summary>
    private void ReassignSlotToPrevious(Job current)
    {
        var previous = JobList.Previous(current);
        if (!_slots.TryReplace(current, previous))
        {
            return;
        }
        Logger.LogTrace("{classification} predecessor job {id} replaced {previousClassification} job {previousId} in scheduled slot",
            previous.Classification, previous.Id, current.Classification, current.Id);
    }
}
