// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.States;

/// <summary>
/// Classification of <see cref="MaterialContainerStateBase"/> subclasses.
/// </summary>
public enum MaterialContainerStateClassification
{
    /// <summary>Material has been requested but not yet announced or registered.</summary>
    Requested = 0,

    /// <summary>Material has been announced as inbound.</summary>
    Inbound = 1,

    /// <summary>Container is registered and in active use.</summary>
    Available = 2,

    /// <summary>Pre-advice for departure was created; container is awaiting pickup.</summary>
    Outbound = 3,

    /// <summary>Container has been deregistered from the system.</summary>
    Deregistered = 4
}