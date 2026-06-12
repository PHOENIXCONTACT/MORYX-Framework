// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// State of a container which has been announced as inbound.
/// </summary>
[DataContract]
public class InboundState : MaterialContainerStateBase
{
    /// <inheritdoc />
    public override MaterialContainerStateClassification Classification => MaterialContainerStateClassification.Inbound;

    /// <summary>
    /// Optional identifier of the announcement.
    /// </summary>
    [DataMember]
    public Guid? AnnouncementId { get; set; }

    /// <summary>
    /// Optional expected arrival of the announced material.
    /// </summary>
    [DataMember]
    public DateTime? ExpectedArrival { get; set; }

    /// <summary>
    /// Indicates whether material related to this announcement was already (partially) registered.
    /// </summary>
    [DataMember]
    public bool IsPartiallyFulfilled { get; set; }

    /// <summary>
    /// Optional cross-reference to a preceding material request.
    /// </summary>
    [DataMember]
    public Guid? RequestReference { get; set; }
}