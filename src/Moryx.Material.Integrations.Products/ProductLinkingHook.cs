// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Specialized <see cref="LinkingHook{TReference, TBusinessObject}"/> base class for product-linking scenarios.
/// Provides strongly typed access to the <see cref="ProductType"/> business objects resolved by the product integration
/// through <see cref="LinkingHook{TReference, TBusinessObject}.Target"/> and
/// <see cref="LinkingHook{TReference, TBusinessObject}.Previous"/>.
/// </summary>
public abstract class ProductLinkingHook : LinkingHook<ProductTypeReference, ProductType>
{
}
