// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.Jobs;

/// <summary>
/// Public API for JobManager
/// </summary>
public interface IJobManagement
{
    /// <summary>
    /// Estimate effort to produce the given amount of a certain product
    /// including estimations for setup and material refill based on the
    /// current machine states. This estimation may change
    /// </summary>
    JobEvaluation Evaluate(IProductRecipe recipe, int amount);

    /// <summary>
    /// Creates multiple jobs at once
    /// </summary>
    Task<IReadOnlyList<Job>> AddAsync(JobCreationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a job by the given id
    /// If this is an inactive job, than the JobHistory will be used to search for the asked job
    /// </summary>
    Job Get(long jobId);

    /// <summary>
    /// Retrieve the full job list
    /// </summary>
    IReadOnlyList<Job> GetAll();

    /// <summary>
    /// Completes the given job.
    /// All running processes will be finished.
    /// </summary>
    /// <param name="job">The reference to the job to complete.</param>
    void Complete(Job job);

    /// <summary>
    /// Stops the given job.
    /// Will interrupt all running processes with an final unmount.
    /// </summary>
    void Abort(Job job);

    /// <summary>
    /// Latest evaluations are outdated and should be evaluated by <see cref="Evaluate(IProductRecipe,int)"/>
    /// </summary>
    event EventHandler EvaluationsOutdated;

    /// <summary>
    /// A jobs progress has changed
    /// </summary>
    event EventHandler<Job> ProgressChanged;

    /// <summary>
    /// Raised whenever a job has changes.
    /// </summary>
    event EventHandler<JobStateChangedEventArgs> StateChanged;
}