// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Material container with the ability to be linked to a <see cref="ProductType"/>.
/// </summary>
/// <remarks>
/// Containers can implement this alongside other linked container interfaces to be linked to multiple business objects.
/// </remarks>
public interface IProductLinkedMaterialContainer : IMaterialContainer
{
    /// <summary>
    /// Link to the product type of this container.
    /// </summary>
    ProductLink ProductLink { get; }
}
