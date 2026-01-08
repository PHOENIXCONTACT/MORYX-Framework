// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.Processes;

/// <summary>
/// Interface for a filtered set of processes
/// </summary>
public interface IProcessChunk : IEnumerable<Process>
{
    /// <summary>
    /// Job the processes in this chunk belong to
    /// </summary>
    Job Job { get; }
}
