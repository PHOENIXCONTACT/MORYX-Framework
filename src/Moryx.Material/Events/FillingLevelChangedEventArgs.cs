// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Events;

/// <summary>
/// Event args for <see cref="IMaterialContainer.FillingLevelChanged"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="FillingLevelChangedEventArgs"/>.
/// </remarks>
public class FillingLevelChangedEventArgs(IMaterialContainer container, decimal oldQuantity, decimal newQuantity) :
    MaterialContainerEventArgs(container)
{
    /// <summary>
    /// Previous quantity.
    /// </summary>
    public decimal OldQuantity { get; } = oldQuantity;

    /// <summary>
    /// New quantity.
    /// </summary>
    public decimal NewQuantity { get; } = newQuantity;
}
