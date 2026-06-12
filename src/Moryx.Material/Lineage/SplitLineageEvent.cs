// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Lineage;

/// <summary>
/// Lineage event recorded when material is split from one source container into one or more destinations.
/// </summary>
[DataContract]
public class SplitLineageEvent : LineageEventBase
{
    /// <summary>
    /// Resource ids of destination containers.
    /// </summary>
    [DataMember]
    public IReadOnlyList<long> DestinationContainerIds { get; set; } = Array.Empty<long>();

    /// <summary>
    /// Quantity transferred from the source.
    /// </summary>
    [DataMember]
    public decimal TransferredQuantity { get; set; }
}
