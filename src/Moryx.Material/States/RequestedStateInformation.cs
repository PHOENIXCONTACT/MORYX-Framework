// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// State information of a virtual container created for a material request.
/// </summary>
[DataContract]
public class RequestedStateInformation : StateInformation
{
    /// <summary>
    /// Optional identifier of the underlying request.
    /// </summary>
    [DataMember]
    [Display(Name = "Request ID", Description = "Unique identifier of the material request.")]
    public string? RequestId { get; set; }

    /// <summary>
    /// Optional expected arrival of the requested material.
    /// </summary>
    [DataMember]
    [Display(Name = "Expected Arrival", Description = "Expected arrival date and time of the requested material (optional).")]
    public DateTimeOffset? ExpectedArrival { get; set; }

    /// <summary>
    /// Indicates whether material related to this request was already (partially) announced or registered.
    /// </summary>
    [DataMember]
    [Display(Name = "Partially Fulfilled", Description = "Indicates whether the request was already partially fulfilled.")]
    public bool IsPartiallyFulfilled { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var details = new List<string>();
        if (!string.IsNullOrWhiteSpace(RequestId))
            details.Add($"Request {RequestId}");
        if (ExpectedArrival.HasValue)
            details.Add($"ETA {ExpectedArrival:yyyy-MM-dd HH:mm}");
        return details.Count > 0 ? $"Requested ({string.Join(", ", details)})" : "Requested";
    }
}
