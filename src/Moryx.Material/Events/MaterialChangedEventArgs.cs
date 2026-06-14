// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Events;

/// <summary>
/// Event args for <see cref="IMaterialContainer.MaterialChanged"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="MaterialChangedEventArgs"/>.
/// </remarks>
public class MaterialChangedEventArgs(IMaterialContainer container, string? oldMaterial, string? newMaterial) :
    MaterialContainerEventArgs(container)
{
    /// <summary>
    /// Previous material reference (may be null).
    /// </summary>
    public string? OldMaterial { get; } = oldMaterial;

    /// <summary>
    /// New material reference (may be null).
    /// </summary>
    public string? NewMaterial { get; } = newMaterial;
}
