// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Lineage;

/// <summary>
/// Base interface for all lineage events recorded by the material management system.
/// </summary>
/// <remarks>
/// Lineage events form the audit trail of material containers across registration,
/// deregistration, splits, merges, and integration-specific links.
/// </remarks>
public interface ILineageEvent
{
    /// <summary>
    /// Unique identifier of this event.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Timestamp at which the event occurred.
    /// </summary>
    DateTime Timestamp { get; }

    /// <summary>
    /// Resource id of the primary container affected by the event.
    /// </summary>
    long ContainerId { get; }
}