// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.Lineage;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Lineage event recorded when a container is linked to an order.
/// </summary>
[DataContract]
public class OrderLinkLineageEvent : LinkLineageEventBase
{
    /// <summary>
    /// Order number that was linked.
    /// </summary>
    [DataMember]
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Optional operation number.
    /// </summary>
    [DataMember]
    public string? OperationNumber { get; set; }
}