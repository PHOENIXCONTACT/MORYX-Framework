// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using MQTTnet;

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Manages storage operations for the <see cref="IManagedMqttClient"/>
/// </summary>
/// <param name="messageStorage"></param>
internal sealed class ManagedMqttClientStorageManager(IManagedMqttClientStorage messageStorage)
{
    /// <summary>
    /// Add message to storage
    /// </summary>
    /// <param name="message">Message to add to storage</param>
    /// <returns></returns>
    public async Task AddAsync(MqttApplicationMessage message, CancellationToken cancellationToken)
    {
        var messages = await messageStorage.LoadQueuedMessagesAsync(cancellationToken);
        messages.Add(message);
        await messageStorage.SaveQueuedMessagesAsync(messages, cancellationToken);
    }

    /// <summary>
    /// Remove message from storage
    /// </summary>
    /// <param name="message">Message to remove from storage</param>
    /// <returns></returns>
    public async Task<bool> RemoveAsync(MqttApplicationMessage message, CancellationToken cancellationToken)
    {
        var messages = await messageStorage.LoadQueuedMessagesAsync(cancellationToken);
        var removed = messages.Remove(message);
        await messageStorage.SaveQueuedMessagesAsync(messages, cancellationToken);
        return removed;
    }

    /// <summary>
    /// Load message that are queued to be sent to the broker.
    /// </summary>
    /// <returns> Messages queued to be sent to the broker</returns>
    public async Task<IList<MqttApplicationMessage>> LoadQueuedMessagesAsync(CancellationToken cancellationToken)
    {
        return await messageStorage.LoadQueuedMessagesAsync(cancellationToken);
    }

    /// <summary>
    /// Save message to be sent to the broker when client re-connect
    /// </summary>
    /// <param name="messages">Messages to save</param>
    /// <returns></returns>
    public async Task SaveQueuedMessagesAsync(IList<MqttApplicationMessage> messages, CancellationToken cancellationToken)
    {
        await messageStorage.SaveQueuedMessagesAsync(messages, cancellationToken);
    }
}