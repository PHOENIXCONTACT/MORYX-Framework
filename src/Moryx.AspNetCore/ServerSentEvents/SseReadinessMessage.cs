// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json;
using System.Net.ServerSentEvents;

namespace Moryx.AspNetCore.ServerSentEvents;

/// <summary>
/// Create a readiness message for an SSE stream.
/// </summary>
/// <param name="isReady">
/// Indicates whether the data saource behind the stream is ready.
/// </param>
/// <param name="SerializerOptions">
/// Options used to serialize the readiness state.
/// </param>
/// <return>
/// An SSE item conatining 
/// </return>
public static class SseReadinessMessage
{
    public static SseItem<string> Create(bool isReady, JsonSerializerOptions serializerOptions)
    {
        var model = new EndpointReadinessModel(isReady);
        var data = JsonSerializer.Serialize(model, serializerOptions);

        return new SseItem<string>(
            data, SseEventTypes.Readiness);
    }
}
