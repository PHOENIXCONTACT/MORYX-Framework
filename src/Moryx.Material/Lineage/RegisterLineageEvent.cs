// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Lineage;

/// <summary>
/// Lineage event recorded when a container is registered (transitions to Available).
/// </summary>
[DataContract]
public class RegisterLineageEvent : LineageEventBase
{
    /// <summary>
    /// Material reference at registration time.
    /// </summary>
    [DataMember]
    public string? Material { get; set; }

    /// <summary>
    /// Quantity at registration time.
    /// </summary>
    [DataMember]
    public decimal Quantity { get; set; }
}