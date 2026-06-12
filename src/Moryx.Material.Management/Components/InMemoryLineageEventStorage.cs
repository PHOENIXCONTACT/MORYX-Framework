// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Lineage;

namespace Moryx.Material.Management.Components;

/// <summary>
/// Default in-memory implementation of <see cref="ILineageEventStorage"/>.
/// </summary>
[Component(LifeCycle.Singleton, typeof(ILineageEventStorage))]
internal class InMemoryLineageEventStorage : ILineageEventStorage
{
    private readonly List<ILineageEvent> _events = new();
    private readonly object _lock = new();

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
            _events.Clear();
        return Task.CompletedTask;
    }

    public Task RecordAsync(ILineageEvent lineageEvent, CancellationToken cancellationToken = default)
    {
        if (lineageEvent == null)
            throw new ArgumentNullException(nameof(lineageEvent));

        lock (_lock)
            _events.Add(lineageEvent);

        Recorded?.Invoke(this, lineageEvent);
        return Task.CompletedTask;
    }

    public IReadOnlyList<ILineageEvent> GetForContainer(long containerId)
    {
        lock (_lock)
            return _events.Where(e => e.ContainerId == containerId)
                          .OrderBy(e => e.Timestamp)
                          .ToArray();
    }

    public IReadOnlyList<ILineageEvent> Query(Func<ILineageEvent, bool> filter)
    {
        lock (_lock)
            return _events.Where(filter).ToArray();
    }

    public event EventHandler<ILineageEvent>? Recorded;
}