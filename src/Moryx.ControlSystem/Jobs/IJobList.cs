// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Jobs;

/// <summary>
/// Public API for internal job list of the process execution
/// </summary>
public interface IJobList
{
    /// <summary>
    /// Return direct previous job
    /// </summary>
    Job Previous(Job reference);

    /// <summary>
    /// Return direct next job
    /// </summary>
    Job Next(Job reference);

    /// <summary>
    /// Enumerate the job list forward starting from a given element
    /// </summary>
    IEnumerable<Job> Forward();

    /// <summary>
    /// Enumerate the job list forward starting from a given element
    /// </summary>
    IEnumerable<Job> Forward(Job startJob);

    /// <summary>
    /// Enumerate the job list in reverse direction
    /// </summary>
    IEnumerable<Job> Backward();

    /// <summary>
    /// Enumerate the job list in reverse direction
    /// </summary>
    IEnumerable<Job> Backward(Job startJob);

    /// <summary>
    /// Raised whenever a job progress was updated
    /// </summary>
    event EventHandler<Job> ProgressChanged;
}