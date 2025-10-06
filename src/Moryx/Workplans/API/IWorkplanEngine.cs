// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Engine processing a workplan using a specific instance. Any progress on the workplan instance is communicated via events.
    /// </summary>
    public interface IWorkplanEngine : IDisposable
    {
        /// <summary>
        /// Workplan instance executed by this engine
        /// </summary>
        IWorkplanInstance ExecutedWorkplan { get; }

        /// <summary>
        /// Context of the executable workplan
        /// </summary>
        IWorkplanContext Context { get; }

        /// <summary>
        /// Start the processing the workplan instance
        /// </summary>
        void Start();

        /// <summary>
        /// Pause execution and create a snapshot of the workplan instance
        /// </summary>
        /// <returns>Snapshot of all places and transitions and their tokens</returns>
        WorkplanSnapshot Pause();

        /// <summary>
        /// Restore a snapshot to resume execution of a previously paused workplan instance
        /// </summary>
        void Restore(WorkplanSnapshot snapshot);

        /// <summary>
        /// Event raised when a transition was triggered
        /// </summary>
        event EventHandler<ITransition> TransitionTriggered;

        /// <summary>
        /// Event raised when the execution of a workplan instance was completed
        /// </summary>
        event EventHandler<IPlace> Completed;
    }

    /// <summary>
    /// Additional interface for <see cref="IWorkplanEngine"/> to monitor the current execution
    /// </summary>
    public interface IMonitoredEngine : IWorkplanEngine
    {
        /// <summary>
        /// Event raised when a token was placed on a place with classification
        /// <see cref="NodeClassification.Intermediate"/>
        /// </summary>
        event EventHandler<IPlace> PlaceReached;
    }
}
