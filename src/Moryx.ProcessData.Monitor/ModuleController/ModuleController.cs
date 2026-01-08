// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ProcessData.Listener;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Monitor;

/// <summary>
/// Module controller of the process data monitor.
/// </summary>
public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IProcessDataMonitor>
{
    /// <inheritdoc />
    public override string Name => "ProcessDataMonitor";

    /// <summary>
    /// Create instance of process data monitor
    /// </summary>
    public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container.SetInstance(ConfigManager);

        Container.LoadComponents<IProcessDataListener>();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<IProcessDataCollector>().Start();
        ActivateFacade(_processDataMonitorFacade);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<IProcessDataCollector>().Stop();
        DeactivateFacade(_processDataMonitorFacade);
        return Task.CompletedTask;
    }

    private readonly ProcessDataMonitorFacade _processDataMonitorFacade = new();
    IProcessDataMonitor IFacadeContainer<IProcessDataMonitor>.Facade => _processDataMonitorFacade;
}