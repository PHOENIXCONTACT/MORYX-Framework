// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Material.Integrations.Products.Integrator;
using Moryx.Material.Linking;
using Moryx.Modules;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Specialized <see cref="LinkingHook{TReference, TBusinessObject}"/> base class for product-linking scenarios.
/// Provides strongly typed access to the <see cref="ProductType"/> business objects resolved by the product integration
/// through <see cref="LinkingHook{TReference, TBusinessObject}.Target"/> and
/// <see cref="LinkingHook{TReference, TBusinessObject}.Previous"/>.
/// </summary>
public abstract class ProductLinkingHook : LinkingHook<ProductTypeReference, ProductType>, IAsyncConfiguredInitializable<ProductLinkingHookConfig>
{
    /// <inheritdoc />
    /// <remarks>
    /// Override to read a derived <see cref="ProductLinkingHookConfig"/> of the hook.
    /// </remarks>
    public virtual Task InitializeAsync(ProductLinkingHookConfig config, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
