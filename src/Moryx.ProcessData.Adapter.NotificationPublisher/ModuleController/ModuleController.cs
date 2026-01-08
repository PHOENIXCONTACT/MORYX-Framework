// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Notifications;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Adapter.NotificationPublisher;

/// <summary>
/// Module controller of the process data monitor adapter.
/// </summary>
public class ModuleController : ServerModuleBase<ModuleConfig>
{
    /// <inheritdoc />
    public override string Name => "PdmNotificationPublisher";

    /// <summary>
    /// NotificationPublisher facade dependency
    /// </summary>
    [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
    public INotificationPublisher NotificationPublisher { get; set; }

    /// <summary>
    /// ProcessDataMonitor facade dependency
    /// </summary>
    [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
    public IProcessDataMonitor ProcessDataMonitor { get; set; }

    /// <summary>
    /// Create instance of notification adapter
    /// </summary>
    public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container.SetInstance(NotificationPublisher)
            .SetInstance(ProcessDataMonitor);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<NotificationPublisherAdapter>().Start();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<NotificationPublisherAdapter>().Stop();
        return Task.CompletedTask;
    }
}