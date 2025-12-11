// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Jobs;

/// <summary>
/// Enumeration representing the status of an allocation token
/// </summary>
public enum AllocationTokenStatus
{
    /// <summary>
    /// Allocation token has been created but is not yet tracked
    /// </summary>
    Created,

    /// <summary>
    /// Allocation token is being tracked with an adjustment hook
    /// </summary>
    Registered,

    /// <summary>
    /// Allocation token has been dropped and is no longer active
    /// </summary>
    Dropped
}
