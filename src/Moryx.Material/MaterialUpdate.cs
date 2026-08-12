// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material;

/// <summary>
/// Describes a change to the material content of an <see cref="IMaterialContainer"/>.
/// </summary>
public class MaterialUpdate()
{
    /// <summary>
    /// Specifies which material properties are updated by this change.
    /// </summary>
    public required UpdateKind Kind { get; set; }

    /// <summary>
    /// New material reference to apply when <see cref="Kind"/> contains <see cref="UpdateKind.MaterialType"/>.
    /// </summary>
    public string? Material { get; set; }

    /// <summary>
    /// New quantity to apply when <see cref="Kind"/> contains <see cref="UpdateKind.FillingLevel"/>.
    /// </summary>
    public double Quantity { get; set; }

    // ToDo: Should the unit be changed via an update as well? If yes do I need an extra kind (propably yes)?
    /// <summary>
    /// Optional unit associated with <see cref="Quantity"/>. Currently informational only.
    /// </summary>
    public string? Unit { get; set; }
}
