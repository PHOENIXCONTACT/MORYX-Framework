// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
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
    internal sealed class SeamlessScheduler : JobSchedulerBase<SeamlessSchedulerConfig>
    {
        private JobSlotWrapper _running;

        private JobSlots _completingSlots;
        private JobSlots _resumeSlots;

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
            var allJobs = jobs.ToList();
            // Option 1: System restarted and we resume jobs
            if (allJobs.Any(j => j.RunningProcesses.Count > 0))
            {
                foreach (var job in allJobs.Where(j => j.RunningProcesses.Count > 0))
                {
                    yield return job;
                }
            }
            // Option 2:System idle or only occupied by completing jobs with no follow up
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
            // Option 3: Running slot occupied and still required
            else
            {
                // We only accept follow-ups of the running job or its follow-ups
                foreach (var job in allJobs)
                {
                    var previous = JobList.Previous(job);
                    if (_running.Target.SameRecipeAs(job) && job.SameRecipeAs(previous))
                        yield return job;
                }
            }
        }

        public override void JobsReady(IEnumerable<Job> startableJobs)
        {
            // If the running slot is currently assigned to a running job, leave this waiting
            if (_running.Target?.Classification <= JobClassification.Running)
                return;

            var allJobs = startableJobs.ToList();
            // If the RunningSlot is currently assigned, with a completing job, we move it
            if (_running.Target?.Classification == JobClassification.Completing && JobList.Next(_running.Target).IsCleanup())
            {
                // Move to completing slots
                MoveToCompleting();
            }

            var first = allJobs[0];
            // If the running slot falls back to a completing group, move it back to running
            var previous = JobList.Previous(first);
            if (first.SameRecipeAs(previous) && previous.Classification == JobClassification.Completing && _completingSlots.TryRelease(previous))
            {
                _completingSlots.TryResize(_completingSlots.Size - 1);
            }

            // Iterate all jobs and assign to running or resume slots
            var last = first;
            foreach (var job in allJobs)
            {
                if (job == first && job.RunningProcesses.Count == 0)
                {
                    // New job takes the running slot
                    _running.Assign(job);
                    RaiseJobScheduled(job);
                }
                else if (job.RunningProcesses.Count > 0)
                {
                    // Previously running jobs are resumed
                    if (job.SameRecipeAs(last) && _resumeSlots.TryReplace(last, job))
                    {
                        // Follow-up to resumed job with slot
                        RaiseJobScheduled(job);
                    }
                    else if (_resumeSlots.TryResize(_resumeSlots.Size + 1) && _resumeSlots.TryAssign(job))
                    {
                        // Provide new resume slot
                        RaiseJobScheduled(job);
                    }
                }
            }
        }

        public override void JobUpdated(Job job, JobClassification classification)
        {
            // After restart we move jobs from resume slots to productive slots
            if (_resumeSlots.HasSlot(job))
            {
                if (classification == JobClassification.Running && _resumeSlots.TryRelease(job))
                {
                    // Resumed job takes the running slot
                    _running.Assign(job);
                }
                else if (classification == JobClassification.Completing && _resumeSlots.TryRelease(job))
                {
                    // Resumed job is completing
                    MoveToCompleting(job, false);
                }

                // Prune resume slots
                _resumeSlots.TryResize(_resumeSlots.Size - _resumeSlots.AvailableSlots);
                if (_resumeSlots.Size == 0 && _running.Target == null)
                    RaiseSlotAvailable(); // If we resumed everything, we can reassign running with new jobs
            }

            // Changes below Completing do not interest us
            if (classification < JobClassification.Completing)
                return;

            // If the completed job was a cleanup, remove the slot
            if (classification == JobClassification.Completed && job.IsCleanup() && _completingSlots.TryRelease(job))
            {
                _completingSlots.TryResize(_completingSlots.Size - 1);
                return;
            }

            // Find the next waiting job
            var next = JobList.Forward(job).FirstOrDefault(n => n.Classification == JobClassification.Waiting);
            if (next == null)
                return;

            if (_running.Target == job && next.SameRecipeAs(job))
            {
                // Next job is a follow-up for the completing job
                _running.Assign(next);
                RaiseJobScheduled(next);
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
                    _running.Assign(next);
                    RaiseJobScheduled(next);
                }
                else if (next.IsCleanupOf(job) && next.IsCleanupOf(JobList.Previous(job)))
                {
                    // Job has the slot, but previous is still running, return the slot
                    _running.Assign(JobList.Previous(job));
                    // If the slot falls back from running to completing, it becomes available
                    RaiseSlotAvailable();
                }
                else if (job.IsPrepare() && next.IsCleanup() || next.IsCleanupOf(job))
                {
                    // We start the clean-up
                    MoveToCompleting(next);
                    RaiseJobScheduled(next);
                    // We have released the running slot
                    RaiseSlotAvailable();
                }
            }
            else if (classification == JobClassification.Completed && next.IsCleanupOf(job))
            {
                // Completing jobs completed in reverse order
                var previous = JobList.Previous(job);
                if (next.IsCleanupOf(previous) && _completingSlots.TryReplace(job, previous))
                {
                    // Assign slot back to previous job
                }
                // A running job from the completing slots has completed, start the cleanup
                else if(_completingSlots.TryReplace(job, next))
                {
                    RaiseJobScheduled(next);
                }
            }
        }

        /// <summary>
        /// Move job on running slot to completing
        /// </summary>
        private void MoveToCompleting(Job job = null, bool clearRunning = true)
        {
            _completingSlots.TryResize(_completingSlots.Size + 1);
            _completingSlots.TryAssign(job ?? _running.Target);
            if (clearRunning)
                _running.Assign(null);
        }
    }
}
