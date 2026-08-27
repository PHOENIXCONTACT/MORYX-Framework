// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Material.Facade;
using Moryx.Material.Integrations.Products.Integrator.Components;
using Moryx.Material.Integrations.Products.Integrator.Facade;
using Moryx.Material.Linking;
using Moryx.Runtime.Modules;

namespace Moryx.Material.Integrations.Products.Integrator;

/// <summary>
/// Module controller of the product integration for material management. This module wires
/// <see cref="IProductLinkedMaterialContainer"/> resources to the product management facade
/// and orchestrates configurable <see cref="ILinkingHook"/> plugins.
/// </summary>
[Display(Name = "Material Management - Product Integration", Description = "Wires product linking semantics into the material management module.")]
public class ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
    : ServerModuleBase<ModuleConfig>(containerFactory, configManager, loggerFactory), IFacadeContainer<IProductIntegration>
{
    /// <inheritdoc />
    public override string Name => "Material Management - Product Integration";

    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Product management facade used to resolve <see cref="ProductType"/> business objects.
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IProductManagement ProductManagement { get; set; }

    /// <summary>
    /// Material management facade used to record lineage events and retrieve relevant
    /// product-linked containers.
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IMaterialManagement MaterialManagement { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container.SetInstance(ProductManagement)
            .SetInstance(MaterialManagement);

        Container.LoadComponents<ILinkingHook>();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await Container.Resolve<ILinkingHookManager>().StartAsync(cancellationToken);
        await Container.Resolve<IProductTypeReferencesPool>().StartAsync(cancellationToken);
        await Container.Resolve<IProductContainerManager>().StartAsync(cancellationToken);
        ActivateFacade(_facade);
    }

    /// <inheritdoc />
    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        DeactivateFacade(_facade);
        await Container.Resolve<IProductContainerManager>().StopAsync(cancellationToken);
        await Container.Resolve<IProductTypeReferencesPool>().StopAsync(cancellationToken);
        await Container.Resolve<ILinkingHookManager>().StopAsync(cancellationToken);
    }

    private readonly ProductIntegrationFacade _facade = new();

    IProductIntegration IFacadeContainer<IProductIntegration>.Facade => _facade;
}