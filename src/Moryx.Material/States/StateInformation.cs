// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// Base class for the lifecycle state information of a <see cref="IMaterialContainer"/>.
/// </summary>
/// <remarks>
/// Subclasses represent the information specific to the different lifecycle stages: <see cref="RequestedStateInformation"/>,
/// <see cref="InboundStateInformation"/>, <see cref="AvailableStateInformation"/>, <see cref="OutboundStateInformation"/>,
/// <see cref="DeregisteredStateInformation"/>.
/// </remarks>
[DataContract]
[KnownType(typeof(RequestedStateInformation))]
[KnownType(typeof(InboundStateInformation))]
[KnownType(typeof(AvailableStateInformation))]
[KnownType(typeof(OutboundStateInformation))]
[KnownType(typeof(DeregisteredStateInformation))]
public abstract class StateInformation
{
    /// <summary>
    /// Timestamp when the state was entered.
    /// </summary>
    [DataMember]
    [Display(Name = "Entered At", Description = "UTC timestamp when the container entered this state.")]
    public DateTimeOffset EnteredAt { get; protected internal set; } = DateTimeOffset.UtcNow;
}
