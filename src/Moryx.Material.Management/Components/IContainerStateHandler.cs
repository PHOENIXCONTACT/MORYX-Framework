// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.States;
using Moryx.Modules;

namespace Moryx.Material.Management.Components;

/// <summary>
/// Coordinates state transitions on <see cref="IMaterialContainer"/> resources, raising
/// the appropriate facade events and recording lineage entries.
/// </summary>
internal interface IContainerStateHandler : IPlugin
{
    /// <summary>
    /// Transitions the container to the given new state, raises events and records lineage.
    /// </summary>
    Task TransitionAsync(IMaterialContainer container, MaterialContainerStateBase newState, CancellationToken cancellationToken = default);

    /// <summary>Raised after a transition completed.</summary>
    event EventHandler<ContainerStateChangedEventArgs>? StateChanged;

    /// <summary>Raised when a container enters Available (registration).</summary>
    event EventHandler<MaterialContainerEventArgs>? ContainerAvailable;

    /// <summary>Raised when a container enters Deregistered.</summary>
    event EventHandler<MaterialContainerEventArgs>? ContainerDeregistered;

    /// <summary>Raised when a container enters Requested.</summary>
    event EventHandler<MaterialContainerEventArgs>? MaterialRequested;

    /// <summary>Raised when a container enters Inbound.</summary>
    event EventHandler<MaterialContainerEventArgs>? MaterialInbound;

    /// <summary>Raised when a container enters Outbound.</summary>
    event EventHandler<MaterialContainerEventArgs>? MaterialOutbound;
}