// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.AspNetCore.Mqtt.Components;
using MQTTnet;

namespace Moryx.AspNetCore.Mqtt.Services;

/// <summary>
/// Handles a given resource event with argument type  <typeparamref name="T"/>
/// </summary>
public partial class ResourceEventHandler<T>(
    IManagedMqttClient client,
    JsonSerializerOptions options,
    string topic,
    string eventName)
{
    /// <summary>
    /// Handles a given event for a resource
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArg"></param>
    public async void InvokeMethod(object sender, T eventArg)
    {
        if (sender is not IResource resource)
        {
            return;
        }

        var messageBuilder = new MqttApplicationMessageBuilder();
        var eventData = eventArg as EventArgs;
        var rawPayload = new ResourceEventPayload
        {
            Resource = new ResourceModel(
                resource is IIdentifiableObject obj
                ? obj.Identity.Identifier
                : resource.Id.ToString()
            , resource.Name),
            Event = eventName,
            EventData = eventData
        };
        messageBuilder.WithTopic(topic);
        messageBuilder.WithPayload(MqttMessageSerialization.GetJsonPayload(rawPayload, options));
        messageBuilder.WithContentType("application/json");
        await client.EnqueueAsync(messageBuilder.Build(), CancellationToken.None);
    }
}
