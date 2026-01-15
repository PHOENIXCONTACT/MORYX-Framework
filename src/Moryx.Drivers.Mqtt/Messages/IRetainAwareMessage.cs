// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.Mqtt.Messages;

/// <summary>
/// Interface for messages send via MQTT to mark that they care about being either Retain or not
/// </summary>
public interface IRetainAwareMessage
{
    /// <summary>
    /// Marks if the mqtt message is retain. Null means using the default value of the topic
    /// </summary>
    public bool? Retain { get; set; }
}

