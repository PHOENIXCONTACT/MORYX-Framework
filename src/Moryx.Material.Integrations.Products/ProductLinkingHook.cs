// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Specialized <see cref="LinkingHook"/> base class for product-linking scenarios. Provides
/// strongly typed access to the <see cref="ProductType"/> business object resolved by the
/// integration manager.
/// </summary>
public abstract class ProductLinkingHook : LinkingHook
{
    /// <summary>
    /// The resolved <see cref="ProductType"/> being linked. <c>null</c> when handling a pure unlink request.
    /// </summary>
    public ProductType? ProductType { protected get; set; }

    /// <summary>
    /// The previously linked <see cref="ProductType"/>, if any. Set when re-linking auto-unlinks first.
    /// </summary>
    public ProductType? PreviousProductType { protected get; set; }
}