// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Lineage;

/// <summary>
/// Lineage event recorded when material from multiple source containers is merged into one destination.
/// </summary>
[DataContract]
public class MergeLineageEvent : LineageEventBase
{
    /// <summary>
    /// Resource ids of source containers.
    /// </summary>
    [DataMember]
    public IReadOnlyList<long> SourceContainerIds { get; set; } = Array.Empty<long>();

    /// <summary>
    /// Quantity received by the destination (i.e. <see cref="ILineageEvent.ContainerId"/>).
    /// </summary>
    [DataMember]
    public decimal ReceivedQuantity { get; set; }
}