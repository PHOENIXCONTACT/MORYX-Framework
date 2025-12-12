// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.Mqtt.Topics;

/// <summary>
/// Configuration value for the JSON format
/// </summary>
public enum JsonFormat
{
    /// <summary>
    /// Format object keys as PascalCase
    /// </summary>
    PascalCase = 0,
    /// <summary>
    /// Format object keys as camelCase
    /// </summary>
    CamelCase = 1
}

