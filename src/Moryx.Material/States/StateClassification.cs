// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.States;

/// <summary>
/// Recognized state classifications within the MORYX material management defining the
/// different stages in the flow of material or a material container through a MORYX system.
/// </summary>
public enum StateClassification
{
    /// <summary>
    /// Container state has not yet been initialized by the lifecycle state machine.
    /// </summary>
    Uninitialized = 0,

    /// <summary>
    /// Material has been requested but not yet announced or registered.
    /// </summary>
    Requested = 1,

    /// <summary>
    /// Material has been announced as inbound.
    /// </summary>
    Inbound = 2,

    /// <summary>
    /// Container is registered and in active use.
    /// </summary>
    Available = 3,

    /// <summary>
    /// Pre-advice for departure was created; container is awaiting pickup.
    /// </summary>
    Outbound = 4,

    /// <summary>
    /// Container has been deregistered from the system.
    /// </summary>
    Deregistered = 5
}
