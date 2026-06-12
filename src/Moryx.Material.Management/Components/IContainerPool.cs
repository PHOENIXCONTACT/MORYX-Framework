// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Material.Management.Components;

/// <summary>
/// Internal pool of <see cref="IMaterialContainer"/> instances tracked by the module.
/// </summary>
internal interface IContainerPool : IAsyncPlugin
{
    /// <summary>Returns all tracked containers.</summary>
    IReadOnlyList<IMaterialContainer> GetAll();

    /// <summary>Returns containers matching the given filter.</summary>
    IReadOnlyList<IMaterialContainer> GetAll(Func<IMaterialContainer, bool> filter);

    /// <summary>Returns a container by resource id.</summary>
    IMaterialContainer? Get(long id);

    /// <summary>Adds a container created via <see cref="IResourceManagement"/>.</summary>
    void Track(IMaterialContainer container);

    /// <summary>Removes a container from the pool.</summary>
    void Untrack(IMaterialContainer container);

    /// <summary>Raised when a container is added.</summary>
    event EventHandler<IMaterialContainer>? ContainerAdded;

    /// <summary>Raised when a container is removed.</summary>
    event EventHandler<IMaterialContainer>? ContainerRemoved;
}