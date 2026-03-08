// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.Mqtt.Topics;

/// <summary>
/// Conveys when properties will be ignored
/// </summary>
public enum JsonIgnoreCondition
{
    /// <see cref="System.Text.Json.Serialization.JsonIgnoreCondition.Never" />
    Never,
    /// <see cref="System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault" />
    WhenWritingDefault,
    /// <see cref="System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull" />
    WhenWritingNull,
}
