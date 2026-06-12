// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.Lineage;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Lineage event recorded when an order link is removed from a container.
/// </summary>
[DataContract]
public class OrderUnlinkLineageEvent : LinkLineageEventBase
{
    /// <summary>
    /// Previously linked order number.
    /// </summary>
    [DataMember]
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Optional operation number.
    /// </summary>
    [DataMember]
    public string? OperationNumber { get; set; }
}