// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Materials;

/// <summary>
/// Public API of resources that represent material containers of different types
/// </summary>
public interface IMaterialContainer : IResource
{
    /// <summary>
    /// Identifier of this material container
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// Remaining instances in the container
    /// </summary>
    int InstanceCount { get; }

    /// <summary>
    /// Cell supplied with material by this container
    /// </summary>
    ICell SuppliedCell { get; }

    /// <summary>
    /// Identifier of the material currently in this container
    /// </summary>
    ProductType ProvidedMaterial { get; }

    /// <summary>
    /// Replace the currently <see cref="ProvidedMaterial"/> or simply update the object reference
    /// </summary>
    void SetMaterial(ProductType material);

    /// <summary>
    /// Event raised when the material
    /// </summary>
    event EventHandler MaterialChanged;

    /// <summary>
    /// Event raised when the filling level changed
    /// </summary>
    event EventHandler FillingLevelChanged;
}