// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.ProcessEngine.Processes;

/// <summary>
/// Access to the ProcessEngines activity pool
/// </summary>
internal interface IActivityDataPool
{
    /// <summary>
    /// Number of processes in the pool
    /// </summary>
    int ProcessCount { get; }

    /// <summary>
    /// Adds new process data to the Pool.
    /// </summary>
    /// <param name="processData"></param>
    void AddProcess(ProcessData processData);

    /// <summary>
    /// Adds a new activity to a process in the Pool.
    /// </summary>
    /// <param name="processData">Process to add the activity to</param>
    /// <param name="activityData">The new activityData.</param>
    void AddActivity(ProcessData processData, ActivityData activityData);

    /// <summary>
    /// Process was modified
    /// </summary>
    /// <param name="processData">Modified process</param>
    /// <param name="newState">The new state of the process</param>
    void UpdateProcess(ProcessData processData, ProcessState newState);

    /// <summary>
    /// A component has modified the activity and wants to inform all others
    /// </summary>
    /// <param name="activityData">The modified activity</param>
    /// <param name="newState">The new state of the activity</param>
    bool TryUpdateActivity(ActivityData activityData, ActivityState newState, Action<ActivityData> updateAction = default);

    /// <summary>
    /// Get all processes from the pool
    /// </summary>
    IReadOnlyList<ProcessData> Processes { get; }

    /// <summary>
    /// Get process by id
    /// </summary>
    ProcessData GetProcess(long id);

    /// <summary>
    /// Get process by id
    /// </summary>
    ProcessData GetProcess(Process process);

    /// <summary>
    /// Get process by condition
    /// </summary>
    ProcessData GetProcess(ProcessReference reference);

    /// <summary>
    /// Get wrapped activity data by business object
    /// </summary>
    ActivityData GetByActivity(Activity wrapped);

    /// <summary>
    /// Find an activity that meets certain criteria
    /// </summary>
    /// <returns>Activity if available, otherwise <value>null</value></returns>
    IReadOnlyList<ActivityData> GetByCondition(Func<ActivityData, bool> predicate);

    /// <summary>
    /// All open activities managed by the pool
    /// </summary>
    IReadOnlyList<ActivityData> GetAllOpen();

    /// <summary>
    /// All open activities managed by the pool of a certain process
    /// </summary>
    IReadOnlyList<ActivityData> GetAllOpen(ProcessData processData);

    /// <summary>
    /// Raised if an process changed its state.
    /// If the process's state reached <c>Finished</c> the process is removed from the Pool after this event was fired.
    /// </summary>
    event EventHandler<ProcessEventArgs> ProcessChanged;

    /// <summary>
    /// Raised if an activity changed its state.
    /// If the activity's state reached <c>Finished</c> the activity is removed from the Pool after this event was fired.
    /// </summary>
    event EventHandler<ActivityEventArgs> ActivityChanged;
}
