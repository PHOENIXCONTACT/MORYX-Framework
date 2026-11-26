// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

/// <summary>
/// Factory to create configured product linking hooks.
/// </summary>
[PluginFactory(typeof(IConfigBasedComponentSelector))]
internal interface IProductLinkingHookFactory
{
    /// <summary>
    /// Create a new hook instance.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="ProductLinkingHook"/> instead of <see cref="ILinkingHook"/>, because the
    /// <see cref="IConfigBasedComponentSelector"/> only passes the config to the hook if the
    /// returned type implements <see cref="Modules.IAsyncConfiguredInitializable{T}"/>.
    /// </remarks>
    Task<ProductLinkingHook> Create(ProductLinkingHookConfig config, CancellationToken cancellationToken);

    /// <summary>
    /// Destroy a hook instance.
    /// </summary>
    void Destroy(ILinkingHook instance);
}