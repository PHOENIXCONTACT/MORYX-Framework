// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;

namespace Moryx.Material;

/// <summary>
/// Request to provision a quantity of material to the system.
/// </summary>
public class MaterialRequest
{
    /// <summary>
    /// Optional unique identifier of the request. Generated if not set.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Material reference (e.g., product number).
    /// </summary>
    public string Material { get; set; } = string.Empty;

    /// <summary>
    /// Requested quantity.
    /// </summary>
    public decimal RequestedQuantity { get; set; }

    /// <summary>
    /// Optional unit of <see cref="RequestedQuantity"/>.
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Optional identity of a specific container being requested.
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