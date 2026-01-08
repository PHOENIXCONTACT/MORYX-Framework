// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.FactoryMonitor.Endpoints.Models;

/// <summary>
/// State of a cell
/// </summary>
public enum CellState
{
    /// <summary>
    /// Waiting for the next activity
    /// </summary>
    Idle,

    /// <summary>
    /// Waiting to get an update from the process engine
    /// </summary>
    Requested,

    /// <summary>
    /// Executing an activity
    /// </summary>
    Running,

    /// <summary>
    /// Not able to work
    /// </summary>
    NotReadyToWork
}