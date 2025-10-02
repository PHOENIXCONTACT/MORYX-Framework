// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Defines strategies for handling overflow of pending MQTT messages in the managed client.
/// </summary>
public enum MqttMessagesOverflowStrategy
{
    /// <summary>
    /// New messages are rejected when the maximum number of pending messages is reached.
    /// </summary>
    RejectNewMessage,

    /// <summary>
    /// The oldest queued message is dropped to make space for the new message when the maximum number of pending messages is reached.
    /// </summary>
    DropOldestQueuedMessage
}