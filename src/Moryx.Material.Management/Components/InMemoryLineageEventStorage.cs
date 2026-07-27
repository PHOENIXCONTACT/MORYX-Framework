// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.Container;
using Moryx.Material.Facade;
using Moryx.Material.Lineage;

namespace Moryx.Material.Management.Components;

// TODO: Add database implementation
[Component(LifeCycle.Singleton, typeof(ILineageEventStorage))]
internal class InMemoryLineageEventStorage : ILineageEventStorage
{
    private readonly List<ILineageEvent> _events = [];
    private readonly Lock _lock = new();

    public required IContainerPool Pool { get; set; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // TODO: Read when lineage storage is correctly implemented Pool.StateChanged += OnContainerStateChanged;
        return Task.CompletedTask;
    }

    private void OnContainerStateChanged(object? sender, ContainerStateChangedEventArgs e)
    {
        var lineage = new StateTransitionLineageEvent
        {
            ContainerId = e.Container.Id,
           // FromClassification = e.PreviousStateInformation,
           // ToClassification = e.NewStateInformation
        };

        // Fire and forget lineage recording
        RecordAsync(lineage, CancellationToken.None);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Pool.StateChanged -= OnContainerStateChanged;
        lock (_lock)
        {
            _events.Clear();
        }

        return Task.CompletedTask;
    }

    public Task RecordAsync(ILineageEvent lineageEvent, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _events.Add(lineageEvent);
        }

        Recorded?.Invoke(this, lineageEvent);
        return Task.CompletedTask;
    }

    public IReadOnlyList<ILineageEvent> GetForContainer(long containerId)
    {
        lock (_lock)
        {
            return _events.Where(e => e.ContainerId == containerId)
                          .OrderBy(e => e.Timestamp)
                          .ToArray();
        }
    }

    public IReadOnlyList<ILineageEvent> Query(Func<ILineageEvent, bool> filter)
    {
        lock (_lock)
        {
            return _events.Where(filter).ToArray();
        }
    }

    public event EventHandler<ILineageEvent>? Recorded;
}
