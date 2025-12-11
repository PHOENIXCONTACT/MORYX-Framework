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
        /// Returns processes currently executed by the process controller
        /// </summary>
        IReadOnlyList<Process> GetRunningProcesses();

        /// <summary>
        /// Returns processes currently executed by the process controller
        /// </summary>
        /// <param name="predicate">Filter for the processes</param>
        IReadOnlyList<Process> GetRunningProcesses(Func<Process, bool> predicate);

        /// <summary>
        /// Retrieve all archived processes for a product instance
        /// </summary>
        Task<IReadOnlyList<Process>> GetArchivedProcessesAsync(ProductInstance productInstance);

        /// <summary>
        /// Retrieve all archived  processes in a certain range
        /// </summary>
        IAsyncEnumerable<IProcessChunk> GetArchivedProcessesAsync(ProcessRequestFilter filterType, DateTime start, DateTime end, long[] jobIds);

        /// <summary>
        /// Possible targets for the process, defined by currently open activities
        /// </summary>
        IReadOnlyList<ICell> Targets(Process process);

        /// <summary>
        /// Possible cells that can execute the activity
        /// </summary>
        IReadOnlyList<ICell> Targets(Activity activity);

        /// <summary>
        /// Report a specific <see cref="ReportAction"/> to have been executed on the <paramref name="process"/>
        /// </summary>
        /// <param name="process">The process to report</param>
        /// <param name="action">The action to perform</param>
        void Report(Process process, ReportAction action);

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
