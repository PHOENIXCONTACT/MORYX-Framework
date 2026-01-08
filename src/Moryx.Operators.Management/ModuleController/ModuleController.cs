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
using Moryx.Users;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Moryx.Operators.Management;

/// <summary>
/// Module controller of the operator management module.
/// </summary>
[Description("Manages operators, with their skills and availabilities.")]
public class ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager dbContextManager) :
    ServerModuleBase<ModuleConfig>(containerFactory, configManager, loggerFactory),
    IFacadeContainer<IOperatorManagement>, IFacadeContainer<IAttendanceManagement>,
    IFacadeContainer<IUserManagement>,
    IFacadeContainer<ISkillManagement>
{
    /// <inheritdoc />
    public override string Name => "Operator Management";

    /// <summary>
    /// Reference to the IResourceManagement managing the IAttendableResources
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IResourceManagement ResourceManagement { get; set; }

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container
            .ActivateDbContexts(dbContextManager)
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
    IUserManagement IFacadeContainer<IUserManagement>.Facade => _facade;
}
