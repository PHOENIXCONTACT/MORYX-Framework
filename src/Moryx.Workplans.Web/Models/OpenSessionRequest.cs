// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// Request type for workplan session 
/// </summary>
public class OpenSessionRequest
{
    /// <inheritdoc cref="OpenSessionRequest.WorkplanId" />
    public long WorkplanId { get; set; }

    /// <inheritdoc cref="OpenSessionRequest.Duplicate" />
    public bool Duplicate { get; set; }
}
