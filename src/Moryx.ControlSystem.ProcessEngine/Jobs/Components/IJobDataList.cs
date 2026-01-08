// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// Public API of the JobList
/// </summary>
internal interface IJobDataList : IPlugin, IEnumerable<IJobData>
{
    /// <summary>
    /// Returns a requested job
    /// </summary>
    /// <returns>The requested job or <c>null</c> if there is no job with the given ID.</returns>
    IJobData Get(long jobId);

    /// <summary>
    /// Restore jobs and extract remaining production jobs
    /// </summary>
    void Restore(IReadOnlyList<IJobData> restoredJobs, Action<ModifiedJobsFragment> saveCallback);

    /// <summary>
    /// Add jobs to the job list
    /// </summary>
    void Add(LinkedList<IJobData> newJobs, JobPosition position, Action<ModifiedJobsFragment> saveCallback);

    /// <summary>
    /// Remove a completed job from the list
    /// </summary>
    void Remove(IJobData completedJob, Action<ModifiedJobsFragment> saveCallback);

    /// <summary>
    /// Return direct previous job
    /// </summary>
    IJobData Previous(IJobData reference);

    /// <summary>
    /// Return direct next job
    /// </summary>
    IJobData Next(IJobData reference);

    /// <summary>
    /// Enumerate the job list forward starting from a given element
    /// </summary>
    IEnumerable<IJobData> Forward(IJobData startJob);

    /// <summary>
    /// Enumerate the job list in reverse direction
    /// </summary>
    IEnumerable<IJobData> Backward();

    /// <summary>
    /// Enumerate the job list in reverse direction
    /// </summary>
    IEnumerable<IJobData> Backward(IJobData startJob);

    /// <summary>
    /// Raised whenever a new job is added to the Job list, whether it is 
    /// a new Job or the Job is recovered from the database on restart.
    /// </summary>
    event EventHandler<IReadOnlyList<IJobData>> Added;

    /// <summary>
    /// Advanced state changed event including previous state
    /// </summary>
    event EventHandler<JobStateEventArgs> StateChanged;

    /// <summary>
    /// Raised whenever a job progress was updated
    /// </summary>
    event EventHandler<IJobData> ProgressChanged;
}