// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Material.Facade;
using Moryx.Material.Integrations.Orders.Integrator.Components;
using Moryx.Material.Linking;
using Moryx.Orders;
using Moryx.Runtime.Modules;

namespace Moryx.Material.Integrations.Orders.Integrator;

/// <summary>
/// Module controller of the order integration for material management. This module wires
/// <see cref="IOrderLinkedMaterialContainer"/> resources to the order management facade and
/// orchestrates configurable <see cref="ILinkingHook"/> plugins.
/// </summary>
[Display(Name = "Material Mánagement - Order Integration", Description = "Wires order linking semantics into the material management module.")]
public class ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
    : ServerModuleBase<ModuleConfig>(containerFactory, configManager, loggerFactory)
{
    /// <inheritdoc />
    public override string Name => "Material Mánagement - Order Integration";

    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Order management facade used to resolve <see cref="Order"/> business objects.
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IOrderManagement OrderManagement { get; set; }

    /// <summary>
    /// Material management facade used to record lineage events and retrieve relevant order linked containers
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IMaterialManagement MaterialManagement { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        _ = Container
            .SetInstance(OrderManagement)
            .SetInstance(MaterialManagement);

        Container.LoadComponents<ILinkingHook>();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await Container.Resolve<ILinkingHookManager>().StartAsync(cancellationToken);
        await Container.Resolve<IOrderReferencesPool>().StartAsync(cancellationToken);
        await Container.Resolve<IOrderContainerManager>().StartAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        await Container.Resolve<IOrderContainerManager>().StopAsync(cancellationToken);
        await Container.Resolve<IOrderReferencesPool>().StopAsync(cancellationToken);
        await Container.Resolve<ILinkingHookManager>().StopAsync(cancellationToken);
    }
}
