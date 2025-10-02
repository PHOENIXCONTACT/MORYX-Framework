// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json;
using MQTTnet;


/// <summary>
/// Provide methods to serialize/deserialize an <see cref="MqttApplicationMessage"/> using the provided
/// <see cref="JsonSerializerOptions"/>
/// </summary>
public class MqttMessageSerialization
{
    /// <summary>
    /// Given the <paramref name="payload"/>, returns a JSON serialized string of the response payload
    /// </summary>
    /// <typeparam name="T">Type of the payload</typeparam>
    /// <param name="payload">The payload</param>
    /// <returns>Returns the JSON string of the payload</returns>k
    public static string GetJsonPayload<T>(T payload, JsonSerializerOptions? options = null)
    {
        options ??= JsonSerializerOptions.Default;
        return JsonSerializer.Serialize(payload, options);
    }

    /// <summary>
    /// Deserialize a given <paramref name="message"/> to <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The target type after deserializing</typeparam>
    /// <param name="message">The message to deserialize</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns a deserialized value <typeparamref name="T"/></returns>
    public static ValueTask<T> DeserializeAsync<T>(MqttApplicationMessage message, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
    {
        options ??= JsonSerializerOptions.Default;
        var payloadString = message.ConvertPayloadToString();
        var payloadStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payloadString));
        return JsonSerializer.DeserializeAsync<T>(payloadStream, options, cancellationToken);
    }

    /// <summary>
    /// Deserialize a given <paramref name="message"/> to <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="returnType">The target type after deserializing</typeparam>
    /// <param name="message">The message to deserialize</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns a deserialized value <typeparamref name="returnType"/></returns>
    public static ValueTask<object> DeserializeAsync(MqttApplicationMessage message, Type returnType, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
    {
        options ??= JsonSerializerOptions.Default;
        var payloadString = message.ConvertPayloadToString();
        var payloadStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payloadString));
        return JsonSerializer.DeserializeAsync(payloadStream, returnType, options, cancellationToken);
    }

    /// <summary>
    /// Deserialize a given <paramref name="message"/> to <typeparamref name="T"/>
    /// </summary>
    /// <param name="message">The message to deserialize</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns a deserialized value as an object.</returns>
    public static ValueTask<object?> DeserializeAsync(MqttApplicationMessage message, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= JsonSerializerOptions.Default;
        var payloadString = message.ConvertPayloadToString();
        var payloadStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payloadString));
        return JsonSerializer.DeserializeAsync<object>(payloadStream, options, cancellationToken);
    }
}
