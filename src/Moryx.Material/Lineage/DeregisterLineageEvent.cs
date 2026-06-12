// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Lineage;

/// <summary>
/// Lineage event recorded when a container is deregistered.
/// </summary>
[DataContract]
public class DeregisterLineageEvent : LineageEventBase
{
    /// <summary>
    /// Final quantity at deregistration.
    /// </summary>
    [DataMember]
    public decimal FinalQuantity { get; set; }
}