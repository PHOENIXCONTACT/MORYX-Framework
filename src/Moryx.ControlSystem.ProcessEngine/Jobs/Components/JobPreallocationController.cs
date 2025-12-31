// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Production;
using Moryx.Logging;
using Moryx.Tools;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// Handler for managing preallocated job allocations and tracking allocation tokens
/// </summary>
[Component(LifeCycle.Singleton, typeof(IJobPreallocationController))]
internal class JobPreallocationController : IJobPreallocationController, ILoggingComponent
{
    #region Dependencies

    public IJobManager JobManager { get; set; }

    public IJobDataList JobList { get; set; }

    public IModuleLogger Logger { get; set; }

    #endregion

    #region Fields

    /// <summary>
    /// Set of actively tracked preallocated jobs
    /// </summary>
    private readonly HashSet<IPreallocatedJobData> _activePreallocations = [];
    private readonly Lock _lock = new();

    #endregion

    /// <inheritdoc />
    public void Start()
    {
        JobList.Added += OnJobsAdded;
    }

    /// <summary>
    /// Checking all new jobs for preallocated jobs to register their allocation tokens.
    /// Also wires the adjustment delegate and subscribes to state changes for monitoring
    /// and freezing the allocation.
    /// </summary>
    private void OnJobsAdded(object sender, IReadOnlyList<IJobData> addedJobs)
    {
        foreach (var jobData in addedJobs.OfType<IPreallocatedJobData>())
        {
            var token = jobData.AllocationToken;
            if (token is null || token.Status > AllocationTokenStatus.Created)
            {
                // For some reason the token is already been used, skip it
                Logger.Log(LogLevel.Debug, "Skipping to track preallocated job {Id}. Another component has already taken to track it.", jobData.Id);
                continue;
            }

            // Wire the adjustment delegate
            var adjustmentDelegate = CreatePreallocationAdjustmentHook(jobData);
            token.RegisterPreallocationAdjustmentHook(adjustmentDelegate);

            // Subscribe to state changes
            jobData.StateChanged += OnJobStateChanged;

            // Add to active allocations
            lock (_lock)
            {
                _activePreallocations.Add(jobData);
            }

            Logger.Log(LogLevel.Debug, "Started to track preallocated job {Id}.", jobData.Id);
        }
    }

    /// <summary>
    /// Creates an adjustment delegate for a preallocated job
    /// </summary>
    private AdjustAllocationDelegate CreatePreallocationAdjustmentHook(IPreallocatedJobData jobData)
    {
        return async deltaAmount =>
        {
            var targetAmount = jobData.Amount + deltaAmount;

            // Check for overflow
            if (targetAmount <= IdShiftGenerator.MaxAmount)
            {
                // Adjust the amount on the preallocated job
                jobData.AdjustAmount(targetAmount);
                return;
            }

            // Calculate new amount of the preallocation job and the overflow amount to create new jobs for
            var newAmount = targetAmount % IdShiftGenerator.MaxAmount;
            var overflowAmount = targetAmount - newAmount;

            // Place new jobs before the current to keep the preallocated job at the back and the allocation alive
            var overflowContext = new JobCreationContext(jobData.Recipe, (uint)overflowAmount).Before(jobData.Job);
            var newJobs = await JobManager.Add(overflowContext);
            // Adjust the amount on the preallocated job
            jobData.AdjustAmount(newAmount);

            Logger.LogDebug("Created job(s) {newJobIds} due to an overflow on the preallocation job {PreallocatedJobId}",
                       string.Join(",", newJobs.Select(j => j.Id)), jobData.Id);
        };
    }

    /// <summary>
    /// Register on state changes of the preallocated job to freeze the allocation when the job starts.
    /// We decided to place this logic here to keep all preallocation related logic in one component,
    /// instead of placing it in the OnEnter method of the <see cref="DispatchedState"/> and <see cref="CompletedState"/>.
    /// </summary>
    private void OnJobStateChanged(object sender, JobStateEventArgs e)
    {
        // As long as the job is waiting we allow changes to the preallocated job
        if (sender is not IPreallocatedJobData jobData || e.CurrentState.Classification <= JobClassification.Waiting)
        {
            return;
        }

        // Afterwards the preallocation is frozen and the adjustment hook is dropped
        jobData.StateChanged -= OnJobStateChanged;
        jobData.FreezePreallocation();
        lock (_lock)
        {
            _activePreallocations.Remove(jobData);
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        JobList.Added -= OnJobsAdded;
        DropTrackedPreallocations();
    }

    private void DropTrackedPreallocations()
    {
        lock (_lock)
        {
            foreach (var job in _activePreallocations)
            {
                job.StateChanged -= OnJobStateChanged;
                job.FreezePreallocation();
            }
            _activePreallocations.Clear();
        }
    }
}
