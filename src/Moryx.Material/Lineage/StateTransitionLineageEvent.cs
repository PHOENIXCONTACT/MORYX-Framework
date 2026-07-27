// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.States;

namespace Moryx.Material.Lineage;

/// <summary>
/// Lineage event recorded for any state transition of the container that does not have a more specific event type.
/// </summary>
[DataContract]
public class StateTransitionLineageEvent : LineageEventBase
{
    /// <summary>
    /// Classification of the previous state, or <c>null</c> if this is the initial state.
    /// </summary>
    [DataMember]
    public StateClassification? FromClassification { get; set; }

    /// <summary>
    /// Classification of the new state.
    /// </summary>
    [DataMember]
    public StateClassification ToClassification { get; set; }
}
