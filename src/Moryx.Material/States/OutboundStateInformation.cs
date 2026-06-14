// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Tools;

namespace Moryx.Material.States;

/// <summary>
/// State information of a container with an active pre-advice for departure.
/// </summary>
[DataContract]
[Display(Name = "Outbound", Description = "Container has an active pre-advice for departure.")]
public class OutboundStateInformation : StateInformation
{
    // ToDo: Reason seems to be the wrong term
    /// <summary>
    /// Reason for the announced departure.
    /// </summary>
    [DataMember]
    [Display(Name = "Departure Reason", Description = "Reason for the container leaving the current location.")]
    public PreAdviceDepartureReason DepartureReason { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var display = typeof(PreAdviceDepartureReason).GetMember(nameof(DepartureReason))
            .Single().GetDisplayName();
        return $"Outbound ({display})";
    }
}

/// <summary>
/// Reason for the departure of a container in <see cref="OutboundStateInformation"/>.
/// </summary>
public enum PreAdviceDepartureReason
{
    /// <summary>Finished goods are leaving the production line.</summary>
    [Display(Name = "Finished Goods")]
    FinishedGoods = 0,

    /// <summary>Unconsumed material is being returned.</summary>
    [Display(Name = "Unused Material")]
    UnusedMaterial = 1,

    /// <summary>Container is being transferred to another location.</summary>
    [Display(Name = "Transfer")]
    Transfer = 2,

    /// <summary>Container content is being scrapped.</summary>
    [Display(Name = "Scrap")]
    Scrap = 3,

    /// <summary>Other / unspecified reason.</summary>
    [Display(Name = "Other")]
    Other = 4
}
