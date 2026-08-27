// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Facade of the product integration module.
/// </summary>
public interface IProductIntegration
{
    /// <summary>
    /// Returns all currently managed product type references.
    /// </summary>
    IReadOnlyList<ProductTypeReference> GetProductReferences();
}