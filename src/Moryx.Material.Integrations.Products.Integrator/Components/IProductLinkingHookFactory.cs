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
    // ToDo: Can I specify this to directly return ProductLinkingHooks?
    /// <summary>
    /// Create a new hook instance.
    /// </summary>
    Task<ILinkingHook> Create(ProductLinkingHookConfig config, CancellationToken cancellationToken);

    /// <summary>
    /// Destroy a hook instance.
    /// </summary>
    void Destroy(ILinkingHook instance);
}