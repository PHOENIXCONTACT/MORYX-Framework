// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Lineage;

namespace Moryx.Material.Facade;

/// <summary>
/// Event arguments raised when a lineage event has been recorded.
/// </summary>
/// <param name="container">Container associated with the recorded lineage event.</param>
/// <param name="lineageEvent">Recorded lineage event.</param>
public class LineageRecordedEventArgs(IMaterialContainer container, ILineageEvent lineageEvent) : MaterialContainerEventArgs(container)
{
    /// <summary>
    /// The recorded lineage event.
    /// </summary>
    public ILineageEvent Event { get; } = lineageEvent;
}
