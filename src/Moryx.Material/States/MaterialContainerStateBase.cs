// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// Base class for the lifecycle state of a <see cref="IMaterialContainer"/>.
/// </summary>
/// <remarks>
/// Subclasses represent specific lifecycle stages: <see cref="RequestedState"/>,
/// <see cref="InboundState"/>, <see cref="AvailableState"/>, <see cref="OutboundState"/>,
/// <see cref="DeregisteredState"/>.
/// </remarks>
[DataContract]
[KnownType(typeof(RequestedState))]
[KnownType(typeof(InboundState))]
[KnownType(typeof(AvailableState))]
[KnownType(typeof(OutboundState))]
[KnownType(typeof(DeregisteredState))]
public abstract class MaterialContainerStateBase
{
    /// <summary>
    /// Classification of the state for cross-cutting evaluations.
    /// </summary>
    [DataMember]
    public abstract MaterialContainerStateClassification Classification { get; }

    /// <summary>
    /// Timestamp when the state was entered.
    /// </summary>
    [DataMember]
    public DateTime EnteredAt { get; protected internal set; } = DateTime.UtcNow;
}