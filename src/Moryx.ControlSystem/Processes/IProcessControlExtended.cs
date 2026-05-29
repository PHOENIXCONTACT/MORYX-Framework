// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Processes;

namespace Moryx.ControlSystem.ProcessEngine;

// ToDo: Merge in MORYX 12
/// <summary>
/// Facade interface to get even more information from the process engine
/// </summary>
public interface IProcessControlExtended : IProcessControl
{
    /// <summary>
    /// Retrieve a process by its id.
    /// </summary>
    Task<Process> LoadArchivedProcessAsync(long id, CancellationToken cancellationToken = default);
}
