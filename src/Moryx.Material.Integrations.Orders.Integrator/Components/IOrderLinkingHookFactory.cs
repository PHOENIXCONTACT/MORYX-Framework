// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Linking;
using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

/// <summary>
/// Factory to create configured order linking hooks
/// </summary>
[PluginFactory(typeof(IConfigBasedComponentSelector))]
internal interface IOrderLinkingHookFactory
{
    /// <summary>
    /// Create a new <see cref="OrderLinkingHook"/> matching the provided <paramref name="config"/>.
    /// </summary>
    Task<OrderLinkingHook> Create(OrderLinkingHookConfig config, IMaterialContainer container,
        LinkingRequest<OrderReference> request, ValidationContext validationContext, Order? target, Order? previous,
        CancellationToken cancellationToken);

    /// <summary>
    /// Destroy the <paramref name="instance"/>
    /// </summary>
    void Destroy(ILinkingHook instance);
}
