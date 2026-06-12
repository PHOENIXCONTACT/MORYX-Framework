// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Integrations.Orders.Integrator.Components;
using Moryx.Modules;

namespace Moryx.Material.Integrations.Orders.Integrator;

[Component(LifeCycle.Singleton)]
internal class ComponentOrchestration : IAsyncPlugin
{
    public ILinkingHookManager LinkingHookManager { get; set; } = null!;
    public IOrderContainerManager OrderContainerManager { get; set; } = null!;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await OrderContainerManager.StartAsync(cancellationToken);
        await LinkingHookManager.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await LinkingHookManager.StopAsync(cancellationToken);
        await OrderContainerManager.StopAsync(cancellationToken);
    }
}