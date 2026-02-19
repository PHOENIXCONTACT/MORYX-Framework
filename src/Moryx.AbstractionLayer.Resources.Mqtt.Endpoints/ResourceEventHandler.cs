// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Text.Json;
using Moryx.AbstractionLayer.Identity;
using Moryx.AspNetCore.Mqtt;
using Moryx.AspNetCore.Mqtt.Components;
using Moryx.AspNetCore.Mqtt.Converters;
using MQTTnet;

namespace Moryx.AbstractionLayer.Resources.Mqtt.Endpoints;

/// <summary>
/// Handles a given resource event with argument type  <typeparamref name="T"/>
/// </summary>
public class ResourceEventHandler<T>(IManagedMqttClient client, JsonSerializerOptions options, string topic, string eventName)
{
    /// <summary>
    /// Handles a given event for a resource
    /// </summary>
    public async void InvokeMethod(object sender, T eventArg)
    {
        try
        {
            if (sender is not IResource resource)
            {
                return;
            }

            var messageBuilder = new MqttApplicationMessageBuilder();
            var eventData = eventArg as EventArgs;

            var resourceIdentifier = resource is IIdentifiableObject obj
                ? obj.Identity.Identifier
                : resource.Id.ToString(CultureInfo.InvariantCulture);

            var rawPayload = new ResourceEventPayload
            {
                Resource = new ResourceModel(resourceIdentifier, resource.Name),
                Event = eventName,
                EventData = eventData
            };
            messageBuilder.WithTopic(topic);
            messageBuilder.WithPayload(MqttMessageSerialization.GetJsonPayload(rawPayload, options));
            messageBuilder.WithContentType("application/json");
            await client.EnqueueAsync(messageBuilder.Build(), CancellationToken.None);
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }
}
