// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Encodings.Web;

namespace Moryx.Drivers.Mqtt.Topics;

/// <summary>
/// Controls how special characters are escaped
/// </summary>
public enum JsonEncoderOption
{
    /// <see cref="JavaScriptEncoder.Default" />
    Default,
    /// <see cref="JavaScriptEncoder.UnsafeRelaxedJsonEscaping" />
    UnsafeRelaxedJsonEscaping
}
