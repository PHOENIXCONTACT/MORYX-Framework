// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Runtime.Endpoints.Modules.Models;

/// <summary>
/// Contract for a config response
/// </summary>
public class Config
{
    /// <summary>
    /// Entry converted configuration
    /// </summary>
    public Entry Root { get; set; }
}