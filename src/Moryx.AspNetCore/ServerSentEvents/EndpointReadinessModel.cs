// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore.ServerSentEvents;

/// <summary>
/// Represent the readiness state of the data source behind an SSE stream.
/// </summary>
/// <param name="IsReady">
/// Indicates whether the data source is ready and available.
/// </param>
public sealed record EndpointReadinessModel(bool IsReady);
