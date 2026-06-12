// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;

#pragma warning disable CS8618

namespace Moryx.Material.Management;

/// <summary>
/// Module controller of the Material Management module.
/// </summary>
[Description("Manages material containers, their lifecycle and lineage events.")]
public class ModuleController(
    IModuleContainerFactory containerFactory,
    IConfigManager configManager,
    ILoggerFactory loggerFactory)
    : ServerModuleBase<ModuleConfig>(containerFactory, configManager, loggerFactory),
      IFacadeContainer<IMaterialManagement>
{
    internal const string ModuleName = "MaterialManagement";

    /// <inheritdoc />
    public override string Name => ModuleName;

    /// <summary>
    /// Resource management is required to discover and manage <see cref="IMaterialContainer"/> resources.
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IResourceManagement ResourceManagement { get; set; }

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container.SetInstance(ResourceManagement);
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