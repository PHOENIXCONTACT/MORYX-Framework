// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Editing.Components
{
    internal interface IWorkplanEditingSession
    {
        /// <summary>
        /// Domain session object
        /// </summary>
        WorkplanSession Session { get; }

        /// <summary>
        /// Add step to the workplan
        /// </summary>
        void AddStep(IWorkplanStep step);

        /// <summary>
        /// Update a workplan step or its position
        /// </summary>
        bool UpdateStep(IWorkplanStep step);

        /// <summary>
        /// Remove a step
        /// </summary>
        bool RemoveStep(long stepId);

        /// <summary>
        /// Add a connector
        /// </summary>
        void AddConnector(IConnector connector);

        /// <summary>
        /// Remove connector
        /// </summary>
        bool RemoveConnector(long connectorId);

        /// <summary>
        /// Connect output of one step with input of another one
        /// </summary>
        void Connect(IWorkplanNode sourceNode, int sourceIndex, IWorkplanNode targetNode, int targetIndex);

        /// <summary>
        /// Connect output of one step with input of another one
        /// </summary>
        void Disconnect(IWorkplanNode sourceNode, int sourceIndex, IWorkplanNode targetNode, int targetIndex);

        /// <summary>
        /// Auto layout a session
        /// </summary>
        void AutoLayout();
    }
}
