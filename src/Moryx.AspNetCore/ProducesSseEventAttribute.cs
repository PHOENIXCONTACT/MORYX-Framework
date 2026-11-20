// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Moryx.AspNetCore;

/// <summary>
/// Augmentation to AspNetCore build in ProducesAttribute to document sse events.
/// Because currently only one payloadType can be attached to a response code in Microsoft.AspNetCore.OpenApi.
/// It generates pseudo response codes >1000 to document them.
/// To have them show up in a generated spec correctly use the SseOperationTransformer defined in Moryx.AspNetCore.OpenApi.
/// It will merge them into the text/event-stream response for code 200.
/// </summary>
/// <param name="eventType"></param>
/// <param name="payloadType"></param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ProducesSseEventAttribute(string eventType, Type payloadType) : ProducesResponseTypeAttribute(payloadType, GetPseudoUniqueKey(eventType), "text/event-stream"), IApiResponseMetadataProvider
{
    /// <summary>
    /// EventType used in the sse message
    /// </summary>
    public string EventType { get; set; } = eventType;

    /// <summary>
    /// Type of the payload beeing send when this EventType is send
    /// </summary>
    public Type PayloadType => base.Type;

    /// <summary>
    /// Returns a hash based id for the eventype that is greater than thousand too stop conflicts with legitamate ResponseCodes
    /// </summary>
    private static int GetPseudoUniqueKey(string eventType)
    {
        var value = (eventType.GetHashCode() % 1_000_000_000) + 1_000_001_000;
        Debug.Assert(value > 1000);
        return value;
    }
}

/// <summary>
/// Helper to use ProducesSseEvent when enums are used as event types
/// </summary>
/// <typeparam name="T">Enum type used to discriminate events</typeparam>
public class ProducesSseEventAttribute<T>(T key, Type responseType) : ProducesSseEventAttribute(key.ToString(), responseType) where T : Enum
{

}
