// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.ControlSystem.Jobs;
using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Component that provides access to running and completed processes
    /// </summary>
    internal interface IProcessArchive : IPlugin
    {
        /// <summary>
        /// Retrieve all processes for a product instance
        /// </summary>
        IReadOnlyList<IProcess> GetProcesses(ProductInstance productInstance);

        /// <summary>
        /// Retrieve all processes in a certain range
        /// </summary>
        /// <param name="filterType">Type of filtering</param>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        /// <param name="jobIds">Optional filter with job ids</param>
        IEnumerable<IProcessChunk> GetProcesses(RequestFilter filterType, DateTime start, DateTime end, long[] jobIds);
    }

    /// <summary>
    /// Interface for a filtered set of processes
    /// </summary>
    internal interface IProcessChunk : IEnumerable<IProcess>
    {
        /// <summary>
        /// Job the processes in this chunk belong to
        /// </summary>
        Job Job { get; }
    }

    /// <summary>
    /// Filter flags that can be combined
    /// </summary>
    internal enum RequestFilter
    {
        /// <summary>
        /// Filter only by start and end
        /// </summary>
        Timed = 1,

        /// <summary>
        /// Filter by given job ids
        /// </summary>
        Jobs = 2
    }
}
