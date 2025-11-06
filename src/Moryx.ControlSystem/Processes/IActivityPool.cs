// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Access to all activities currently running or prepared by the process control
    /// </summary>
    public interface IActivityPool
    {
        /// <summary>
        /// Number of processes in the pool
        /// </summary>
        int ProcessCount { get; }

        /// <summary>
        /// Get all processes from the pool
        /// </summary>
        IReadOnlyList<IProcess> Processes { get; }

        /// <summary>
        /// Get process by id
        /// </summary>
        IProcess GetProcess(long id);

        /// <summary>
        /// Find an activity that meets certain criteria
        /// </summary>
        /// <returns>Activity if available, otherwise <value>null</value></returns>
        IReadOnlyList<IActivity> GetByCondition(Func<IActivity, bool> predicate);

        /// <summary>
        /// All open activities managed by the pool
        /// </summary>
        IReadOnlyList<IActivity> GetAllOpen();

        /// <summary>
        /// All open activities managed by the pool of a certain process
        /// </summary>
        IReadOnlyList<IActivity> GetAllOpen(IProcess process);

        /// <summary>
        /// Raised if an process changed its state.
        /// If the process's state reached <c>Finished</c> the process is removed from the Pool after this event was fired.
        /// </summary>
        event EventHandler<ProcessUpdatedEventArgs> ProcessUpdated;

        /// <summary>
        /// Raised if an activity changed its state.
        /// If the activity's state reached <c>Finished</c> the activity is removed from the Pool after this event was fired.
        /// </summary>
        event EventHandler<ActivityUpdatedEventArgs> ActivityUpdated;
    }
}
