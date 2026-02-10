// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using MQTTnet;

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Storage interface for managed MQTT client message queuing
/// </summary>
public interface IManagedMqttClientStorage
{
    /// <summary>
    /// Load message that are queued to be sent to the broker.
    /// </summary>
    /// <returns></returns>
    Task<IList<MqttApplicationMessage>> LoadQueuedMessagesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Save message to be sent to the broker when client re-connect
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    Task SaveQueuedMessagesAsync(IList<MqttApplicationMessage> messages, CancellationToken cancellationToken);
}