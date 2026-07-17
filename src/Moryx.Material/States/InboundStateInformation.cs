// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// State information of a container which has been announced as inbound.
/// </summary>
[DataContract]
public class InboundStateInformation : StateInformation
{
    /// <summary>
    /// Optional identifier of the announcement.
    /// </summary>
    [DataMember]
    [Display(Name = "Announcement ID", Description = "Unique identifier of the material announcement (optional).")]
    public string? AnnouncementId { get; set; }

    /// <summary>
    /// Optional expected arrival of the announced material.
    /// </summary>
    [DataMember]
    [Display(Name = "Expected Arrival", Description = "Expected arrival date and time of the inbound material (optional).")]
    public DateTimeOffset? ExpectedArrival { get; set; }

    /// <summary>
    /// Indicates whether material related to this announcement was already (partially) registered.
    /// </summary>
    [DataMember]
    [Display(Name = "Partially Fulfilled", Description = "Indicates whether the announcement was already partially fulfilled.")]
    public bool IsPartiallyFulfilled { get; set; }

    /// <summary>
    /// Optional cross-reference to <see cref="RequestedStateInformation.RequestId"/>.
    /// </summary>
    [DataMember]
    [Display(Name = "Request Reference", Description = "Unique identifier of the related material request, if any (optional).")]
    public string? RequestReference { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var details = new List<string>();
        if (!string.IsNullOrWhiteSpace(AnnouncementId))
            details.Add($"Announcement {AnnouncementId}");
        if (ExpectedArrival.HasValue)
            details.Add($"ETA {ExpectedArrival:yyyy-MM-dd HH:mm}");
        return details.Count > 0 ? $"Inbound ({string.Join(", ", details)})" : "Inbound";
    }
}
