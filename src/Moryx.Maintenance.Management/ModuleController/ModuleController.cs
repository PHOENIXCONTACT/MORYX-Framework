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
    public IResourceManagement ResourceManager { get; set; }

    #endregion
    /// <summary>
    /// Code executed on start up and after service was stopped and should be started again
    /// </summary>
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container
            .ActivateDbContexts(DbContextManager)
            .SetInstance(ResourceManager);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Code executed after OnInitialize
    /// </summary>
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<IMaintenanceManager>().Start();

        ActivateFacade(_facade);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Code executed when service is stopped
    /// </summary>
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        DeactivateFacade(_facade);

        Container.Resolve<IMaintenanceManager>().Stop();
        return Task.CompletedTask;
    }

    private readonly MaintenanceManagementFacade _facade = default!;
    public IMaintenanceManagement Facade => _facade;
}
