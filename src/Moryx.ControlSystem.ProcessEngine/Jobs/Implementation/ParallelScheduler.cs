// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// - Configured number of slots
    /// - Slots are assigned from Prepare to clean-up
    /// - Jobs of the same recipe can be added to slots
    /// - Slots are only released, when all jobs of the slot are completed
    /// - There can be more than one running job
    /// - Slots are not transferred between different recipes. They are released and reassigned
    /// </summary>
    [ExpectedConfig(typeof(ParallelSchedulerConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IJobScheduler), Name = nameof(ParallelScheduler))]
    internal class ParallelScheduler : JobSchedulerBase<ParallelSchedulerConfig>
    {
        private JobSlots _slots;

        /// <inheritdoc />
        public override void Initialize(JobSchedulerConfig config)
        {
            base.Initialize(config);

            _slots = new JobSlots(Config.MaxActiveJobs);
        }

        public override IEnumerable<Job> SchedulableJobs(IEnumerable<Job> jobs)
        {
            // Create collection to work on
            var schedulableJobs = jobs.ToList();

            Job last = null;
            var slots = _slots.AvailableSlots;
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
                else if (slots > 0)
                {
                    // This occupies a slot
                    slots--;
                    index++;
                }
                else
                {
                    // We can not schedule this
                    schedulableJobs.Remove(job);
                }

                last = job;
            }

            return schedulableJobs;
        }

        public override void JobsReady(IEnumerable<Job> startableJobs)
        {
            foreach (var job in startableJobs)
            {
                var last = JobList.Previous(job);

                // If the last job was a setup, declare dependency for production job
                if (last.IsPrepareOf(job))
                {
                    AddDependency(job, last);
                }

                // If this is a setup, declare dependency on previous job
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

                // If the job has no dependencies and could be assigned to a slot we start it
                if (Dependencies(job).Count == 0 &&
                    (followUp ? _slots.TryReplace(last, job) : _slots.TryAssign(job)))
                {
                    RaiseJobScheduled(job);
                }
            }
        }

        public override void JobUpdated(Job job, JobClassification classification)
        {
            // Changes below Completing do not interest us
            if (classification < JobClassification.Completing)
                return;

            // If the completed job was a cleanup, release the slot
            if (classification == JobClassification.Completed && job.IsCleanup() && _slots.TryRelease(job))
            {
                RaiseSlotAvailable();
                return;
            }

            // Check the direct next job for a follow-up
            var next = JobList.Next(job);
            if (next == null || next.Classification != JobClassification.Waiting)
            {
                // If the completed job was a setup, but the following jobs are already gone (e.g. aborted), release the slot
                if (classification == JobClassification.Completed && job.IsSetup() && _slots.TryRelease(job))
                {
                    RaiseSlotAvailable();
                }
                return;
            }

            if (classification == JobClassification.Completing)
            {
                // Flag if this job is a follow-up for the completing job
                var followUp = next.SameRecipeAs(job) && RemoveDependency(next, job);
                if (followUp && _slots.TryReplace(job, next))
                    RaiseJobScheduled(next);
            }
            else
            {
                // Remove possible dependency on next job
                RemoveDependency(next, job);

                if (job.IsPrepareOf(next) && _slots.TryReplace(job, next))
                {
                    // If the completed job was a prepare, start its production target
                    RaiseJobScheduled(next);
                }
                else if (job.IsPrepare() && next.IsCleanup() && _slots.TryReplace(job, next))
                {
                    // The production job was aborted during prepare
                    RaiseJobScheduled(next);
                }
                else if (next.IsCleanupOf(job) && next.IsCleanupOf(JobList.Previous(job)))
                {
                    // Job has the slot, but previous is still running, return the slot
                    _slots.TryReplace(job, JobList.Previous(job));
                }
                else if (next.IsCleanupOf(job) && _slots.TryReplace(job, next))
                {
                    // We completed the last production job and start the clean-up
                    RaiseJobScheduled(next);
                }
            }
        }
    }
}

