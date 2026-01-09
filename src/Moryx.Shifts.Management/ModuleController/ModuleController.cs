// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Operators;
using Moryx.Runtime.Modules;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Moryx.Shifts.Management;

/// <summary>
/// Module controller of the operator management module.
/// </summary>
[Description("Manages shifts")]
public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IShiftManagement>
{
    /// <inheritdoc />
    public override string Name => "Shift Management";

    /// <summary>
    /// Generic component to manage database contexts
    /// </summary>
    private readonly IDbContextManager _dbContextManager;

    public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager dbContextManager)
        : base(containerFactory, configManager, loggerFactory)
    {
        _dbContextManager = dbContextManager;
    }

    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IResourceManagement ResourceManagement { get; set; }

    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IOperatorManagement OperatorManagement { get; set; }

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container
            .ActivateDbContexts(_dbContextManager)
            .SetInstance(ResourceManagement)
            .SetInstance(OperatorManagement);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<IShiftStorage>().Start();
        Container.Resolve<IShiftManager>().Start();

        ActivateFacade(_facade);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        DeactivateFacade(_facade);

        Container.Resolve<IShiftManager>().Stop();
        Container.Resolve<IShiftStorage>().Stop();
        return Task.CompletedTask;
    }

    private readonly ShiftManagementFacade _facade = new();

    IShiftManagement IFacadeContainer<IShiftManagement>.Facade => _facade;
}
