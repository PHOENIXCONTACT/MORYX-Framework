// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;

namespace Moryx.ControlSystem.Jobs;

/// <summary>
/// Optional extension interface for <see cref="Job"/>
/// </summary>
public interface IPredictiveJob
{
    /// <summary>
    /// Processes that are predicted to fail based on workplan and current progress
    /// </summary>
    IReadOnlyList<Process> PredictedFailures { get; }
}