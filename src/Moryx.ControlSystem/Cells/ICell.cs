// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Interface for all cells
    /// </summary>
    public interface ICell : IResource
    {
        /// <summary>
        /// Called if the process engine  was attached to production cells.
        /// Can return currently active sessions within the cell
        /// </summary>
        IEnumerable<Session> ProcessEngineAttached(ProcessEngineContext context);

        /// <summary>
        /// Called if the process engine was detached from production cells.
        /// Can return currently active sessions within the cell
        /// </summary>
        IEnumerable<Session> ProcessEngineDetached();

        /// <summary>
        /// Start an activity in the cell
        /// </summary>
        void StartActivity(ActivityStart activityStart);

        /// <summary>
        /// A process is aborting and an activity currently handled by the cell
        /// is affected by this.
        /// </summary>
        void ProcessAborting(Activity affectedActivity);

        /// <summary>
        /// Callback from the process engine, that the sequence was completed
        /// </summary>
        void SequenceCompleted(SequenceCompleted completed);

        /// <summary>
        /// Event raised when the cell is ready to start working
        /// </summary>
        event EventHandler<ReadyToWork> ReadyToWork;

        /// <summary>
        /// Event raised when the cell is no longer ready to work
        /// </summary>
        event EventHandler<NotReadyToWork> NotReadyToWork;

        /// <summary>
        /// Event raised when the resource has completed the activity
        /// </summary>
        event EventHandler<ActivityCompleted> ActivityCompleted;
    }
}
