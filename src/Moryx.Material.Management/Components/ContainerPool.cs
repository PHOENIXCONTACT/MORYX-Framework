// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;

namespace Moryx.Material.Management.Components;

[Component(LifeCycle.Singleton, typeof(IContainerPool))]
internal class ContainerPool : IContainerPool
{
    private readonly ConcurrentDictionary<long, IMaterialContainer> _containers = new();

    public IResourceManagement ResourceManagement { get; set; } = null!;

    public IReadOnlyList<IMaterialContainer> GetAll() => _containers.Values.ToArray();

    public IReadOnlyList<IMaterialContainer> GetAll(Func<IMaterialContainer, bool> filter) =>
        _containers.Values.Where(filter).ToArray();

    public IMaterialContainer? Get(long id) =>
        _containers.TryGetValue(id, out var container) ? container : null;

    public void Track(IMaterialContainer container)
    {
        if (_containers.TryAdd(container.Id, container))
            ContainerAdded?.Invoke(this, container);
    }

    public void Untrack(IMaterialContainer container)
    {
        if (_containers.TryRemove(container.Id, out var removed))
            ContainerRemoved?.Invoke(this, removed);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ResourceManagement.ResourceAdded += OnResourceAdded;
        ResourceManagement.ResourceRemoved += OnResourceRemoved;

        // Track existing containers known to the resource graph
        foreach (var container in ResourceManagement.GetResources<IMaterialContainer>())
            Track(container);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        ResourceManagement.ResourceAdded -= OnResourceAdded;
        ResourceManagement.ResourceRemoved -= OnResourceRemoved;

        _containers.Clear();
        return Task.CompletedTask;
    }

    private void OnResourceAdded(object? sender, IResource resource)
    {
        if (resource is IMaterialContainer container)
            Track(container);
    }

    private void OnResourceRemoved(object? sender, IResource resource)
    {
        if (resource is IMaterialContainer container)
            Untrack(container);
    }

    public event EventHandler<IMaterialContainer>? ContainerAdded;
    public event EventHandler<IMaterialContainer>? ContainerRemoved;
}