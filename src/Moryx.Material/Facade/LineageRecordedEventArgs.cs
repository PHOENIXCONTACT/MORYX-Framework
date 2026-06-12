// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Lineage;

namespace Moryx.Material;

/// <summary>
/// Event args raised when a lineage event has been recorded.
/// </summary>
public class LineageRecordedEventArgs : EventArgs
{
    /// <summary>
    /// The recorded lineage event.
    /// </summary>
    public ILineageEvent Event { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public LineageRecordedEventArgs(ILineageEvent lineageEvent)
    {
        Event = lineageEvent;
    }
}