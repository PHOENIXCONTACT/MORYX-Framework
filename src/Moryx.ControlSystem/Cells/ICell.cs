// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Interface for all cells
    /// </summary>
    public interface ICell : IResource, IControlSystemBound
    {
        /// <summary>
        /// Start an activity in the cell
        /// </summary>
        void StartActivity(ActivityStart activityStart);

        /// <summary>
        /// A process is aborting and an activity currently handled by the cell
        /// is affected by this.
        /// </summary>
        void ProcessAborting(IActivity affectedActivity);

        /// <summary>
        /// Callback from the control system, that the sequence was completed
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
