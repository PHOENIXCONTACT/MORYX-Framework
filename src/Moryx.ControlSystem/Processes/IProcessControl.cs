// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
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
        /// Retrieve all archived processes for a product instance
        /// </summary>
        /// <param name="productInstance">Product instance to select the processes</param>
        Task<IReadOnlyList<IProcess>> GetArchivedProcesses(ProductInstance productInstance);

        /// <summary>
        /// Retrieve all archived processes in a certain range
        /// </summary>
        /// <param name="filterType">Type of filtering</param>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        /// <param name="jobIds">Optional filter with job ids</param>
        IAsyncEnumerable<IProcessChunk> GetArchivedProcesses(ProcessRequestFilter filterType, DateTime start, DateTime end, long[] jobIds);

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
