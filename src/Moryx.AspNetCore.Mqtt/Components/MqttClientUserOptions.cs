// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json;
using MQTTnet.Protocol;

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// User defined options for the MqttClient
/// </summary>
public class MqttClientUserOptions
{
    /// <summary>
    /// Options for the Json Serializer
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();

    /// <summary>
    /// Config for the Mqtt client connection
    /// </summary>
    public MqttConnectionConfig Connection { get; set; } = new();

    /// <summary>
    /// Store message while the client is disconnected from the broker
    /// </summary>
    public IManagedMqttClientStorage MessageStorage { get; set; } = new InMemoryManagedMqttStorage();

    /// <summary>
    /// Limit the amount of message to store inside the <see cref="MessageStorage"/> while
    /// the client is disconnected
    /// </summary>
    public int MaxPendingMessages { get; set; } = 200;

    /// <summary>
    /// Strategy to use when the <see cref="MaxPendingMessages"/> is reached
    /// </summary>
    public MqttMessagesOverflowStrategy PendingMessageStrategy { get; set; } = MqttMessagesOverflowStrategy.DropOldestQueuedMessage;

    /// <summary>
    /// Default topic strategy for subscriptions
    /// </summary>
    public TopicStrategy DefaultTopicStrategy { get; set; } = new TopicStrategy();
}

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