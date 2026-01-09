// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Processes;

/// <summary>
/// Filter flags that can be combined
/// </summary>
public enum ProcessRequestFilter
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
