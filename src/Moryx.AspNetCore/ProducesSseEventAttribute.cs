// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore;

/// <summary>
/// Augmentation to AspNetCore build in ProducesAttribute to document sse events.
/// Need to map to the openapi spec explicitly
/// </summary>
/// <param name="eventType"></param>
/// <param name="payloadType"></param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ProducesSseEventAttribute(string? eventType, Type payloadType) : Attribute
{
    public string EventType { get; set; } = eventType;
    public Type PayloadType { get; set; } = payloadType;

    public string Description { get; set; }
}

/// <summary>
/// Helper to use ProducesSseEvent when enums are used as event types
/// </summary>
/// <typeparam name="T">Enum type used to discriminate events</typeparam>
public class ProducesSseEventAttribute<T>(T key, Type responseType) : ProducesSseEventAttribute(key.ToString(), responseType) where T : Enum
{

}
