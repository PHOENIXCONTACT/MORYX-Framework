// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;

namespace Moryx.Material;

/// <summary>
/// Announcement that material is inbound to the system.
/// </summary>
public class MaterialAnnouncement
{
    /// <summary>
    /// Optional unique identifier of the announcement. Generated if not set.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Optional cross-reference to an existing <see cref="MaterialRequest"/>.
    /// </summary>
    public Guid? RequestReference { get; set; }

    /// <summary>
    /// Material reference. May be omitted if a request is referenced.
    /// </summary>
    public string? Material { get; set; }

    /// <summary>
    /// Announced quantity.
    /// </summary>
    public decimal AnnouncedQuantity { get; set; }

    /// <summary>
    /// Optional unit of <see cref="AnnouncedQuantity"/>.
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Optional identity of a specific announced container.
    /// </summary>
    public IIdentity? ContainerIdentity { get; set; }

    /// <summary>
    /// Optional expected arrival.
    /// </summary>
    public DateTime? ExpectedArrival { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}