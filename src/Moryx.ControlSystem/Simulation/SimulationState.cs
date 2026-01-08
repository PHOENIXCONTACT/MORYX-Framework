// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Simulation;

/// <summary>
/// State the <see cref="ISimulationDriver"/> currently has or represents
/// </summary>
public enum SimulationState
{
    /// <summary>
    /// Device is offline
    /// </summary>
    Offline,
    /// <summary>
    /// Device is connected, but not ready
    /// </summary>
    StandBy,
    /// <summary>
    /// Device is ready, but currently inactive
    /// </summary>
    Idle,
    /// <summary>
    /// Workpiece has arrived and parameters were requested
    /// </summary>
    Requested,
    /// <summary>
    /// Cell has received instructions and executed the process
    /// </summary>
    Executing
}