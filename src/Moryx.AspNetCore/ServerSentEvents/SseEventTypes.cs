// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore.ServerSentEvents;

/// <summary>
/// Contains common event types used in server-sent event streams.
/// </summary>
public static class SseEventTypes
{
    /// <summary>
    /// Event type used to communicate endpoint readiness.
    /// </summary>
    public const string Readiness = "readiness";
}
