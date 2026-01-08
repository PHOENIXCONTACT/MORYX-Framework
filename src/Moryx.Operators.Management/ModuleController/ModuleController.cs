// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Runtime.Modules;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Attendances;
using Moryx.Operators.Skills;

namespace Moryx.Operators.Management;

/// <summary>
/// Module controller of the operator management module.
/// </summary>
/// <inheritdoc/>
[Description("Manages operators, with their skills and availabilities.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager dbContextManager) :
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    ServerModuleBase<ModuleConfig>(containerFactory, configManager, loggerFactory),
    IFacadeContainer<IOperatorManagement>, IFacadeContainer<IAttendanceManagement>,
    IFacadeContainer<ISkillManagement>
{
    /// <summary>
    /// The module's name.
    /// </summary>
    public const string ModuleName = "Operator Management";

    /// <inheritdoc />
    public override string Name => ModuleName;

    /// <summary>
    /// Reference to the IResourceManagement managing the IAttendableResources
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IResourceManagement ResourceManagement { get; set; }

    /// <summary>
    /// Generic component to manage database contexts
    /// </summary>
    public IDbContextManager DbContextManager { get; } = dbContextManager;

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container
            .ActivateDbContexts(DbContextManager)
            .SetInstance(ResourceManagement);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<IOperatorManager>().Start();
        Container.Resolve<IAttendanceManager>().Start();
        Container.Resolve<ISkillStorage>().Start();
        Container.Resolve<ISkillManager>().Start();

        ActivateFacade(_facade);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        DeactivateFacade(_facade);

        Container.Resolve<ISkillManager>().Stop();
        Container.Resolve<ISkillStorage>().Stop();
        Container.Resolve<IAttendanceManager>().Stop();
        Container.Resolve<IOperatorManager>().Stop();
        return Task.CompletedTask;
    }

    private readonly OperatorManagementFacade _facade = new();

    IOperatorManagement IFacadeContainer<IOperatorManagement>.Facade => _facade;
    IAttendanceManagement IFacadeContainer<IAttendanceManagement>.Facade => _facade;
    ISkillManagement IFacadeContainer<ISkillManagement>.Facade => _facade;
}
