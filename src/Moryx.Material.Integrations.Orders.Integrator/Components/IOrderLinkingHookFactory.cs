// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

/// <summary>
/// Factory to create configured order linking hooks
/// </summary>
[PluginFactory(typeof(IConfigBasedComponentSelector))]
internal interface IOrderLinkingHookFactory
{
    // ToDo Can I specify this to directly return OrderLinkingHooks?
    /// <summary>
    /// Create new importer
    /// </summary>
    Task<ILinkingHook> Create(OrderLinkingHookConfig config, CancellationToken cancellationToken);

    /// <summary>
    /// Destroy an importer
    /// </summary>
    void Destroy(ILinkingHook instance);
}
