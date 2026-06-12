// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// State of a container with an active pre-advice for departure.
/// </summary>
[DataContract]
public class OutboundState : MaterialContainerStateBase
{
    /// <inheritdoc />
    public override MaterialContainerStateClassification Classification => MaterialContainerStateClassification.Outbound;

    /// <summary>
    /// Reason for the announced departure.
    /// </summary>
    [DataMember]
    public PreAdviceDepartureReason DepartureReason { get; set; }
}

/// <summary>
/// Reason for the departure of a container in <see cref="OutboundState"/>.
/// </summary>
public enum PreAdviceDepartureReason
{
    /// <summary>Finished goods are leaving the production line.</summary>
    FinishedGoods = 0,

    /// <summary>Unconsumed material is being returned.</summary>
    UnusedMaterial = 1,

    /// <summary>Container is being transferred to another location.</summary>
    Transfer = 2,

    /// <summary>Container content is being scrapped.</summary>
    Scrap = 3,

    /// <summary>Other / unspecified reason.</summary>
    Other = 4
}