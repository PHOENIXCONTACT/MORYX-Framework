// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Endpoints.Common.Response;

/// <summary>
/// Response model for the server time
/// </summary>
public class ServerTimeResponse
{
    /// <summary>
    /// Server time as string
    /// </summary>
    public string ServerTime { get; set; }
}