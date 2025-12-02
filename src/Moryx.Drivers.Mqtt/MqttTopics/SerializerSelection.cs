// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.Mqtt.MqttTopics;

/// <summary>
/// Selects which serializer library should be used for json serialization
/// </summary>
public enum SerializerSelection
{
    /// <summary>
    /// Use System.Text.Json for serialization
    /// </summary>
    SystemTextJson = 0,
    /// <summary>
    /// Use newtonsoft.json for serialization
    /// </summary>
    [Obsolete("Newtonsoft.Json support will be remove soon. See https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/future/docs/articles/adr/0002-system-text-json.md")]
    NewtonsoftJson = 1,
}

