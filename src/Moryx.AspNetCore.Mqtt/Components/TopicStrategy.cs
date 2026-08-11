// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Strategy for topic subscriptions
/// </summary>
public class TopicStrategy
{
    /// <summary>
    /// Gets or sets a value indicating whether the sender will not receive its own published application messages. MQTT 5.0.0+ feature.
    /// </summary>
    public bool NoLocal { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether messages are retained as published or not. MQTT 5.0.0+ feature.
    /// </summary>
    public bool RetainAsPublished { get; set; }
}
