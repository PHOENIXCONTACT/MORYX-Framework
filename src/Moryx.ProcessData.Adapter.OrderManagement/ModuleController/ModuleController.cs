// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Orders;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Adapter.OrderManagement;

/// <summary>
/// Module controller of the process data monitor adapter.
/// </summary>
public class ModuleController : ServerModuleBase<ModuleConfig>
{
    /// <inheritdoc />
    public override string Name => "PdmOrderManagement";

    /// <summary>
    /// ProcessControl facade dependency
    /// </summary>
    [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
    public IOrderManagement OrderManagement { get; set; }

    /// <summary>
    /// ProcessDataMonitor facade dependency
    /// </summary>
    [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
    public IProcessDataMonitor ProcessDataMonitor { get; set; }

    /// <summary>
    /// Create order adapter for process data
    /// </summary>
    public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container.SetInstance(OrderManagement)
            .SetInstance(ProcessDataMonitor);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<OrderManagementAdapter>().Start();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<OrderManagementAdapter>().Stop();
        return Task.CompletedTask;
    }
}