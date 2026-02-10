// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using MQTTnet;

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Storage for the <see cref="IManagedMqttClient"/>, implements the <see cref="IManagedMqttClientStorage"/>.
/// Stores message when the <see cref="IManagedMqttClient"/> lost connection to the broker.
/// </summary>
internal class InMemoryManagedMqttStorage : IManagedMqttClientStorage
{
    private readonly ConcurrentQueue<MqttApplicationMessage> _messages = new();

    /// <summary>
    /// Load message that are queued to be sent to the broker.
    /// </summary>
    /// <returns></returns>
    public Task<IList<MqttApplicationMessage>> LoadQueuedMessagesAsync(CancellationToken cancellationToken)
        => Task.FromResult<IList<MqttApplicationMessage>>([.. _messages]);

    /// <summary>
    /// Save message to be sent to the broker when client re-connect
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public Task SaveQueuedMessagesAsync(IList<MqttApplicationMessage> messages, CancellationToken cancellationToken)
    {
        _messages.Clear();
        foreach (var message in messages)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            _messages.Enqueue(message);
        }
        return Task.CompletedTask;
    }
}
