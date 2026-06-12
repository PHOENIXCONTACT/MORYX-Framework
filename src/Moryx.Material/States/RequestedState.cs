// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// State of a "virtual" container created in response to a material request.
/// </summary>
[DataContract]
public class RequestedState : MaterialContainerStateBase
{
    /// <inheritdoc />
    public override MaterialContainerStateClassification Classification => MaterialContainerStateClassification.Requested;

    /// <summary>
    /// Optional identifier of the underlying request.
    /// </summary>
    [DataMember]
    public Guid? RequestId { get; set; }

    /// <summary>
    /// Optional expected arrival of the requested material.
    /// </summary>
    [DataMember]
    public DateTime? ExpectedArrival { get; set; }

    /// <summary>
    /// Indicates whether material related to this request was already (partially) announced or registered.
    /// </summary>
    [DataMember]
    public bool IsPartiallyFulfilled { get; set; }
}