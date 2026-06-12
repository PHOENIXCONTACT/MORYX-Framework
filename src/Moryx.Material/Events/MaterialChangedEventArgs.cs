// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material;

/// <summary>
/// Event args for <see cref="IMaterialContainer.MaterialChanged"/>.
/// </summary>
public class MaterialChangedEventArgs : EventArgs
{
    /// <summary>
    /// The container which changed.
    /// </summary>
    public IMaterialContainer Container { get; }

    /// <summary>
    /// Previous material reference (may be null).
    /// </summary>
    public string? OldMaterial { get; }

    /// <summary>
    /// New material reference (may be null).
    /// </summary>
    public string? NewMaterial { get; }

    /// <summary>
    /// Creates a new instance of <see cref="MaterialChangedEventArgs"/>.
    /// </summary>
    public MaterialChangedEventArgs(IMaterialContainer container, string? oldMaterial, string? newMaterial)
    {
        Container = container;
        OldMaterial = oldMaterial;
        NewMaterial = newMaterial;
    }
}