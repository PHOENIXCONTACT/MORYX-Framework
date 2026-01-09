// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ControlSystem.Processes;
using Moryx.Runtime.Modules;
using System.ComponentModel;

namespace Moryx.ControlSystem.Simulator;

/// <summary>
/// The main module class for the Simulation.
/// </summary>
[Description("Module to handle orders provided by several plugins e.g. Hydra or Web.")]
public class ModuleController : ServerModuleBase<ModuleConfig>
{
    /// <summary>
    /// Name of this module
    /// </summary>
    public override string Name => "MachineSimulator";

    /// <summary>
    /// Resource management to access machines
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true)]
    public IResourceManagement ResourceManagement { get; set; }

    /// <summary>
    /// Resource management to access machines
    /// </summary>
    [RequiredModuleApi(IsStartDependency = true)]
    public IProcessControl ProcessControl { get; set; }

    public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

    #region State transition

    /// <summary>
    /// Code executed on start up and after service was stopped and should be started again
    /// </summary>
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        // Register required modules
        Container
            .SetInstance(ResourceManagement)
            .SetInstance(ProcessControl);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Code executed after OnInitialize
    /// </summary>
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<IProcessSimulator>().Start();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Code executed when service is stopped
    /// </summary>
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<IProcessSimulator>().Stop();
        return Task.CompletedTask;
    }

    #endregion
}
