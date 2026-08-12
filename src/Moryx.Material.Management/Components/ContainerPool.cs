// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Material.Facade;
using Moryx.Material.States;
using Moryx.Tools;

namespace Moryx.Material.Management.Components;

// ToDo: Add logging for tracked containers, i.e. log state changes 
[Component(LifeCycle.Singleton, typeof(IContainerPool))]
internal class ContainerPool : IContainerPool
{
    private readonly ConcurrentDictionary<long, IMaterialContainer> _containers = new();

    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IModuleLogger Logger { get; set; }

    public IResourceManagement ResourceManagement { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    #region Lifecycle

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ResourceManagement.ResourceAdded += OnResourceAdded;
        ResourceManagement.ResourceRemoved += OnResourceRemoved;

        ResourceManagement.GetResources<IMaterialContainer>().ForEach(Track);

        return Task.CompletedTask;
    }

    private void OnResourceAdded(object? sender, IResource resource)
    {
        if (resource is not IMaterialContainer container)
        {
            return;
        }

        Track(container);

        if (container.StateInformation is StateInformation state)
        {
            StateChanged?.Invoke(this, new ContainerStateChangedEventArgs(container, default, state));
        }
    }

    private void Track(IMaterialContainer container)
    {
        if (_containers.TryAdd(container.Id, container))
        {
            container.MaterialUpdated += OnMaterialUpdated;
            container.StateChanged += OnStateChange;
        }
    }

    private void OnMaterialUpdated(object? sender, MaterialUpdatedEventArgs e)
    {
        if (sender is null)
        {
            Logger.LogWarning("An {materialcontainer} raises {event} events without registering itself as sender. Dropping event...",
                nameof(IMaterialContainer), nameof(IMaterialContainer.MaterialUpdated));
            return;
        }

        var update = new ContainerUpdatedEventArgs((IMaterialContainer)sender)
        {
            Kind = e.Kind,
            NewMaterial = e.NewMaterial,
            OldMaterial = e.OldMaterial,
            NewQuantity = e.NewQuantity,
            OldQuantity = e.OldQuantity
        };
        ContainerUpdated?.Invoke(this, update);
    }

    private void OnStateChange(object? sender, StateChangedEventArgs e)
    {
        if (sender is null)
        {
            Logger.LogWarning("An {materialcontainer} raises {event} events without registering itself as sender. Dropping event...",
                nameof(IMaterialContainer), nameof(IMaterialContainer.StateChanged));
            return;
        }

        var update = new ContainerStateChangedEventArgs((IMaterialContainer)sender, e.PreviousStateInformation, e.NewStateInformation);
        StateChanged?.Invoke(this, update);
    }

    private void OnResourceRemoved(object? sender, IResource resource)
    {
        if (resource is not IMaterialContainer container)
        {
            return;
        }

        Untrack(container);

        if (container.StateInformation is not DeregisteredStateInformation)
        {
            var finalState = new ContainerStateChangedEventArgs(container, container.StateInformation, new DeregisteredStateInformation());
            StateChanged?.Invoke(this, finalState);
        }
    }

    private void Untrack(IMaterialContainer container)
    {
        if (_containers.TryRemove(container.Id, out var cachedContainer))
        {
            cachedContainer.MaterialUpdated += OnMaterialUpdated;
            cachedContainer.StateChanged += OnStateChange;
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ResourceManagement.ResourceAdded -= OnResourceAdded;
        ResourceManagement.ResourceRemoved -= OnResourceRemoved;

        _containers.Values.ForEach(Untrack);
        _containers.Clear();

        return Task.CompletedTask;
    }

    #endregion

    #region IContainerPool

    public IReadOnlyList<IMaterialContainer> GetAll() => [.. _containers.Values];

    public IReadOnlyList<IMaterialContainer> GetAll(Func<IMaterialContainer, bool> filter) =>
        [.. _containers.Values.Where(filter)];

    public IMaterialContainer? Get(long id) =>
        _containers.TryGetValue(id, out var container) ? container : null;

    public event EventHandler<ContainerUpdatedEventArgs>? ContainerUpdated;

    public event EventHandler<ContainerStateChangedEventArgs>? StateChanged;

    #endregion
}
