// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Events;

/// <summary>
/// Event arguments for updates to the material content of an <see cref="IMaterialContainer"/>.
/// </summary>
/// <param name="container">Container whose material content was updated.</param>
public class MaterialUpdatedEventArgs(IMaterialContainer container) :
    MaterialContainerEventArgs(container)
{
    /// <summary>
    /// Describes which material properties changed.
    /// </summary>
    public required UpdateKind Kind { get; set; }

    /// <summary>
    /// Previous material reference (may be null).
    /// </summary>
    public string? OldMaterial { get; set; }

    /// <summary>
    /// New material reference (may be null).
    /// </summary>
    public string? NewMaterial { get; set; }

    /// <summary>
    /// Previous quantity.
    /// </summary>
    public double? OldQuantity { get; set; }

    /// <summary>
    /// New quantity.
    /// </summary>
    public double? NewQuantity { get; set; }
}
