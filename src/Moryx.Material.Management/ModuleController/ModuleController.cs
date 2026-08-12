// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Material.Facade;
using Moryx.Runtime.Modules;

namespace Moryx.Material.Management;

/// <summary>
/// Module controller of the Material Management module.
/// </summary>
[Display(Name = "Material Management", Description = "Manages material containers, their lifecycle and lineage events.")]
public class ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager,
    ILoggerFactory loggerFactory)
    : ServerModuleBase<ModuleConfig>(containerFactory, configManager, loggerFactory),
      IFacadeContainer<IMaterialManagement>
{
    /// <inheritdoc />
    public override string Name => "Material Management";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Resource management is required to discover and manage <see cref="IMaterialContainer"/> resources.
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IResourceManagement ResourceManagement { get; set; }

    /// <summary>
    /// Resource type tree is required to discover creatable <see cref="IMaterialContainer"/> resource types.
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IResourceTypeTree ResourceTypes { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container.SetInstance(ResourceManagement)
            .SetInstance(ResourceTypes);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await Container.Resolve<ComponentOrchestration>().StartAsync(cancellationToken);
        ActivateFacade(_facade);
    }

    /// <inheritdoc />
    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        DeactivateFacade(_facade);
        await Container.Resolve<ComponentOrchestration>().StopAsync(cancellationToken);
    }

    private readonly MaterialManagementFacade _facade = new();

    IMaterialManagement IFacadeContainer<IMaterialManagement>.Facade => _facade;
}
