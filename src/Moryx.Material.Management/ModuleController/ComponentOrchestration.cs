// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Management.Components;
using Moryx.Modules;

namespace Moryx.Material.Management;

[Component(LifeCycle.Singleton)]
internal class ComponentOrchestration : IAsyncPlugin
{
    #region Dependencies

    public IContainerPool ContainerPool { get; set; }

    public ILineageEventStorage LineageStorage { get; set; }

    public IMaterialFlowHandler StateHandler { get; set; }

    public IFulfillmentMatcher FulfillmentMatcher { get; set; }

    public ModuleConfig Config { get; set; }

    #endregion

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await LineageStorage.StartAsync(cancellationToken);
        await ContainerPool.StartAsync(cancellationToken);
        StateHandler.Start();
        FulfillmentMatcher.Start();
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        FulfillmentMatcher.Stop();
        StateHandler.Stop();
        await ContainerPool.StopAsync(cancellationToken);
        await LineageStorage.StopAsync(cancellationToken);
    }
}