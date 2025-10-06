// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Facade interface to get more information from the control system
    /// </summary>
    public interface IProcessControl
    {
        /// <summary>
        /// Processes currently executed by the process controller
        /// </summary>
        IReadOnlyList<IProcess> RunningProcesses { get; }

        /// <summary>
        /// Retrieve all processes for a product instance
        /// </summary>
        IReadOnlyList<IProcess> GetProcesses(ProductInstance productInstance);

        /// <summary>
        /// Possible targets for the process, defined by currently open activities
        /// </summary>
        IReadOnlyList<ICell> Targets(IProcess process);

        /// <summary>
        /// Possible cells that can execute the activity
        /// </summary>
        IReadOnlyList<ICell> Targets(IActivity activity);

        /// <summary>
        /// A process has changed
        /// </summary>
        event EventHandler<ProcessUpdatedEventArgs> ProcessUpdated;

        /// <summary>
        /// An activity has changed
        /// </summary>
        event EventHandler<ActivityUpdatedEventArgs> ActivityUpdated;
    }
}
