// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;
using Moryx.Material.Facade;
using Moryx.Modules;

namespace Moryx.Material.Management.Components;

/// <summary>
/// Internal pool of <see cref="IMaterialContainer"/> instances tracked by the module.
/// </summary>
internal interface IContainerPool : IAsyncPlugin, ILoggingComponent
{
    /// <summary>Returns all tracked containers.</summary>
    IReadOnlyList<IMaterialContainer> GetAll();

    /// <summary>Returns containers matching the given filter.</summary>
    IReadOnlyList<IMaterialContainer> GetAll(Func<IMaterialContainer, bool> filter);

    /// <summary>Returns a container by resource id.</summary>
    IMaterialContainer? Get(long id);

    /// <summary>Raised a material was updated.</summary>
    event EventHandler<ContainerUpdatedEventArgs>? ContainerUpdated;

    /// <summary>Raised after a transition completed.</summary>
    event EventHandler<ContainerStateChangedEventArgs>? StateChanged;
}
