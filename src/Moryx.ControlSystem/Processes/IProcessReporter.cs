// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Specialized cell that can detect and publish process events
    /// </summary>
    public interface IProcessReporter : IResource
    {
        /// <summary>
        /// Event raised, when execution of the process failed outside
        /// of defined activity results
        /// </summary>
        event EventHandler<IProcess> ProcessBroken;

        /// <summary>
        /// Event raised when the process failed and was already removed from the system
        /// </summary>
        event EventHandler<IProcess> ProcessRemoved;
    }
}
