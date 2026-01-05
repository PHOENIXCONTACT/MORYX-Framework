// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Maintenance.Management.Components;
using Moryx.Maintenance.Management.Facade;
using Moryx.Model;
using Moryx.Runtime.Modules;

namespace Moryx.Maintenance.Management.ModuleController;

/// <summary>
/// Module controller of the maintenance management module.
/// </summary>
/// <inheritdoc/>
[Description("Manages Maintenance for all IMaintainableResource.")]
public class ModuleController(
    IModuleContainerFactory containerFactory,
    IConfigManager configManager,
    ILoggerFactory loggerFactory,
    IDbContextManager dbContextManager
    )
    : ServerModuleBase<ModuleConfig>(containerFactory, configManager, loggerFactory),
      IFacadeContainer<IMaintenanceManagement>
{

    private const string ModuleName = "MaintenanceManagement";

    /// <summary>
    /// Name of this module
    /// </summary>
    public override string Name => ModuleName;

    #region Dependencies

    /// <summary>
    /// Generic component to manage database contexts
    /// </summary>
    public IDbContextManager DbContextManager { get; } = dbContextManager;

    [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
    public IResourceManagement? ResourceManager { get; set; }

    #endregion

    protected override void OnInitialize()
    {
        Container
            .ActivateDbContexts(DbContextManager)
            .SetInstance(ResourceManager);
    }

    protected override void OnStart()
    {
        Container.Resolve<IMaintenanceManager>().Start();

        ActivateFacade(_facade);
    }

    protected override void OnStop()
    {
        DeactivateFacade(_facade);

        Container.Resolve<IMaintenanceManager>().Stop();
    }

    private readonly MaintenanceManagementFacade _facade = new();
    public IMaintenanceManagement Facade => _facade;
}
