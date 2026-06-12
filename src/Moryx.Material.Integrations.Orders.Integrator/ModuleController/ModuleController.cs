// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Material.Linking;
using Moryx.Runtime.Modules;
using Moryx.Orders;

#pragma warning disable CS8618

namespace Moryx.Material.Integrations.Orders.Integrator;

/// <summary>
/// Module controller of the order integration for material management. This module wires
/// <see cref="IOrderLinkedMaterialContainer"/> resources to the order management facade and
/// orchestrates configurable <see cref="LinkingHook"/> plugins.
/// </summary>
[Description("Wires order linking semantics into the material management module.")]
public class ModuleController(
    IModuleContainerFactory containerFactory,
    IConfigManager configManager,
    ILoggerFactory loggerFactory)
    : ServerModuleBase<ModuleConfig>(containerFactory, configManager, loggerFactory)
{
    internal const string ModuleName = "MaterialOrderIntegrator";

    /// <inheritdoc />
    public override string Name => ModuleName;

    /// <summary>
    /// Order management facade used to resolve <see cref="Order"/> business objects.
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IOrderManagement OrderManagement { get; set; }

    /// <summary>
    /// Material management facade used to record lineage events.
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IMaterialManagement MaterialManagement { get; set; }

    /// <summary>
    /// Resource management used to discover and observe order-linked containers.
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IResourceManagement ResourceManagement { get; set; }

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container
            .SetInstance(OrderManagement)
            .SetInstance(MaterialManagement)
            .SetInstance(ResourceManagement);

        Container.LoadComponents<LinkingHook>();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        return Container.Resolve<ComponentOrchestration>().StartAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        return Container.Resolve<ComponentOrchestration>().StopAsync(cancellationToken);
    }
}