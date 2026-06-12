// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

/// <summary>
/// Plugin factory for <see cref="LinkingHook"/> instances. Hooks are created transiently
/// per linking request following MORYX's name-based plugin selection convention.
/// </summary>
[PluginFactory(typeof(INameBasedComponentSelector))]
internal interface ILinkingHookFactory
{
    /// <summary>
    /// Creates a hook by its component name.
    /// </summary>
    LinkingHook Create(string name);

    /// <summary>
    /// Releases a previously created hook (transient lifecycle cleanup).
    /// </summary>
    void Destroy(LinkingHook hook);
}