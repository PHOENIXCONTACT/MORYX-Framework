// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Lineage;
using Moryx.Modules;

namespace Moryx.Material.Management.Components;

/// <summary>
/// Storage component for lineage events.
/// </summary>
/// <remarks>
/// In this skeleton implementation lineage events are kept in memory. The interface is
/// designed so a future EF Core implementation can be substituted without facade changes.
/// </remarks>
internal interface ILineageEventStorage : IAsyncPlugin
{
    /// <summary>
    /// Persists a lineage event.
    /// </summary>
    Task RecordAsync(ILineageEvent lineageEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all events for the given container, ordered by timestamp ascending.
    /// </summary>
    IReadOnlyList<ILineageEvent> GetForContainer(long containerId);

    /// <summary>
    /// Returns events matching the given filter.
    /// </summary>
    IReadOnlyList<ILineageEvent> Query(Func<ILineageEvent, bool> filter);

    /// <summary>
    /// Raised after an event was recorded.
    /// </summary>
    event EventHandler<ILineageEvent>? Recorded;
}