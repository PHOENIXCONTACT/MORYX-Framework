// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Workflows
{
    /// <summary>
    /// Engine instance processing the workflow. Any progress on the workflow is communicated via events.
    /// </summary>
    public interface IWorkflowEngine : IDisposable
    {
        /// <summary>
        /// Workflow instance executed by this engine instance
        /// </summary>
        IWorkflow ExecutedWorkflow { get; }

        /// <summary>
        /// Context of the executable workflow
        /// </summary>
        IWorkplanContext Context { get; }

        /// <summary>
        /// Start the workflow
        /// </summary>
        void Start();

        /// <summary>
        /// Pause execution and create a snapshot of the workplan
        /// </summary>
        /// <returns>Snapshot of all places and transitions and their tokens</returns>
        WorkflowSnapshot Pause();

        /// <summary>
        /// Restore a snapshot to resume execution of a previously paused workflow
        /// </summary>
        void Restore(WorkflowSnapshot snapshot);

        /// <summary>
        /// Event raised when a transition was triggered
        /// </summary>
        event EventHandler<ITransition> TransitionTriggered;

        /// <summary>
        /// Event raised when workflow was completed
        /// </summary>
        event EventHandler<IPlace> Completed; 
    }

    /// <summary>
    /// Additional interface for <see cref="IWorkflowEngine"/> to monitor the current execution
    /// </summary>
    public interface IMonitoredEngine : IWorkflowEngine
    {
        /// <summary>
        /// Event raised when a token was placed on a place with classification
        /// <see cref="NodeClassification.Intermediate"/>
        /// </summary>
        event EventHandler<IPlace> PlaceReached;
    }
}
