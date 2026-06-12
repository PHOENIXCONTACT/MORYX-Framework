// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material;

/// <summary>
/// Event args for <see cref="IMaterialContainer.FillingLevelChanged"/>.
/// </summary>
public class FillingLevelChangedEventArgs : EventArgs
{
    /// <summary>
    /// The container which changed.
    /// </summary>
    public IMaterialContainer Container { get; }

    /// <summary>
    /// Previous quantity.
    /// </summary>
    public decimal OldQuantity { get; }

    /// <summary>
    /// New quantity.
    /// </summary>
    public decimal NewQuantity { get; }

    /// <summary>
    /// Creates a new instance of <see cref="FillingLevelChangedEventArgs"/>.
    /// </summary>
    public FillingLevelChangedEventArgs(IMaterialContainer container, decimal oldQuantity, decimal newQuantity)
    {
        Container = container;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
    }
}