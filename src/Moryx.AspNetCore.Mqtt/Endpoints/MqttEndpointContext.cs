// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Routing;
using MQTTnet;
using System.Text.Json;
namespace Moryx.AspNetCore.Mqtt.Endpoints;

/// <summary>
/// Encapsulates all Mqtt-specific information about an individual Mqtt message (request).
/// </summary>
/// <param name="request">The incoming message request</param>
/// <param name="topicParameterValues">Values of the Parameters inside the topic</param>
/// <param name="deserializeAsyncDelegate">Delegate to deserialize the message</param>
public class MqttEndpointContext(MqttApplicationMessage request, RouteValueDictionary topicParameterValues, Func<MqttApplicationMessage, CancellationToken, ValueTask<object?>> deserializeAsyncDelegate)
{
    /// <summary>
    /// Message for the current endpoint
    /// </summary>
    public MqttApplicationMessage RequestMessage => request;

    /// <summary>
    /// Value for parameter provided in the topic template i.e value for 'name' in /my-topic/{name:string}
    /// </summary>
    public RouteValueDictionary ParameterValues => topicParameterValues;

    /// <summary>
    /// Gets the Body of the <see cref="RequestMessage"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T?> RequestBody<T>()
      => (T?)await deserializeAsyncDelegate(RequestMessage, CancellationToken.None);

    /// <summary>
    /// Gets the body from the <see cref="RequestMessage"/>
    /// </summary>
    /// <returns>The request body as a <see cref="JsonElement"/> </returns>
    public async Task<JsonElement?> RequestBodyObject()
       => (JsonElement?)await deserializeAsyncDelegate(RequestMessage, CancellationToken.None);

    /// <summary>
    /// Given a <paramref name="propertyName"/>, returns the value matching the property name from the <see cref="RequestMessage"/> body
    /// </summary>
    /// <typeparam name="T">Type of the the property</typeparam>
    /// <param name="propertyName">Name of the property</param>
    /// <returns>Value of the property</returns>
    public async Task<T?> FromBody<T>(string propertyName)
        => (T?)await FromBody(propertyName, typeof(T));

    /// <summary>
    /// Given a <paramref name="propertyName"/> and <paramref name="type"/>, returns the value from the RequestMessage body
    /// </summary>
    /// <param name="type">Type of the the property</param>
    /// <param name="propertyName">Name of the property</param>
    /// <returns>Value of the property</returns>
    public async Task<object?> FromBody(string propertyName, Type type)
    {
        var jsonElement = await RequestBodyObject();
        if (jsonElement == null)
        {
            return null;
        }

        if (jsonElement.Value.TryGetProperty(propertyName, out var property))
        {
            return JsonSerializer.Deserialize(property, type);
        }

        return null;
    }

    /// <summary>
    /// Given a <paramref name="parameterName"/>, returns the matching value from the current <see cref="RequestMessage"/> topic.
    /// </summary>
    /// <typeparam name="T">Type of the parameter</typeparam>
    /// <param name="parameterName">Name of the parameter as defined in the topic template</param>
    /// <returns></returns>
    public T FromParameterValues<T>(string parameterName)
        where T : notnull
    {
        ParameterValues.TryGetValue(parameterName, out var value);
        return value == null
            ? default
            : (T?)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
    }
}
