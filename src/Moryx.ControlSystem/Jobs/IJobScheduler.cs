// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// The public API of the JobScheduler.
    /// </summary>
    public interface IJobScheduler : IConfiguredPlugin<JobSchedulerConfig>
    {
        /// <summary>
        /// Let the scheduler filter the next schedulable jobs
        /// </summary>
        IEnumerable<Job> SchedulableJobs(IEnumerable<Job> jobs);

        /// <summary>
        /// Inform scheduler, that jobs were flagged ready and can be started now
        /// </summary>
        void JobsReady(IEnumerable<Job> startableJobs);

        /// <summary>
        /// Job was updated
        /// </summary>
        void JobUpdated(Job job, JobClassification classification);

        /// <summary>
        /// Raised if the scheduler has a slot for the job
        /// </summary>
        event EventHandler SlotAvailable;

        /// <summary>
        /// Event raised when a job is scheduled and can be started now
        /// </summary>
        event EventHandler<Job> Scheduled;

        /// <summary>
        /// Event raised when a job is suspended and shall be stopped
        /// </summary>
        event EventHandler<Job> Suspended;
    }
}
