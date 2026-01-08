// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.ControlSystem.Processes;
using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Processes;

/// <summary>
/// Component that provides access to running and completed processes
/// </summary>
internal interface IProcessArchive : IPlugin
{
    /// <summary>
    /// Retrieve all processes for a product instance
    /// </summary>
    Task<IReadOnlyList<Process>> GetProcesses(ProductInstance productInstance, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve all processes in a certain range
    /// </summary>
    /// <param name="filterType">Type of filtering</param>
    /// <param name="start">Start time</param>
    /// <param name="end">End time</param>
    /// <param name="jobIds">Optional filter with job ids</param>
    IAsyncEnumerable<IProcessChunk> GetProcesses(ProcessRequestFilter filterType, DateTime start, DateTime end, long[] jobIds, CancellationToken cancellationToken);
}