// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData.Configuration;

/// <summary>
/// Enum for selecting target for binding values
/// </summary>
public enum ValueTarget
{
    /// <summary>
    /// Target of the value should be measurement fields
    /// </summary>
    Field,

    /// <summary>
    /// Target of the value should be measurement tags
    /// </summary>
    Tag
}