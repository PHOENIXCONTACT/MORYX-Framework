// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Runtime.Modules;
using System.ComponentModel;

namespace Moryx.Notifications.Publisher;

/// <summary>
/// Module controller of the notifications module
/// </summary>
[Description("Module to handle publishing of notifications from the applications.")]
public class ModuleController : ServerModuleBase<ModuleConfig>,
    IFacadeContainer<INotificationPublisher>
{
    /// <inheritdoc />
    public override string Name => "NotificationPublisher";

    /// <summary>
    /// Manager for creating configured DbContext instances
    /// </summary>
    private readonly IDbContextManager _dbContextManager;

    /// <summary>
    /// Notification facades from other modules
    /// </summary>
    [RequiredModuleApi]
    public INotificationSource[] NotificationSources { get; set; }

    /// <summary>
    /// Creates a new instance of the module
    /// </summary>
    public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager dbContextManager)
        : base(containerFactory, configManager, loggerFactory)
    {
        _dbContextManager = dbContextManager;
    }

    /// <inheritdoc />
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        // Register global components
        Container.ActivateDbContexts(_dbContextManager);

        // Load additional processors
        Container.LoadComponents<INotificationProcessor>();

        // Register sources
        foreach (var source in NotificationSources)
            Container.SetInstance(source, source.Name);

        // Initialize components
        Container.Resolve<INotificationManager>().Initialize();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Container.Resolve<INotificationManager>().Start();

        ActivateFacade(_notificationPublisherFacade);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        DeactivateFacade(_notificationPublisherFacade);

        Container.Resolve<INotificationManager>().Stop();
        return Task.CompletedTask;
    }

    private readonly NotificationPublisherFacade _notificationPublisherFacade = new();

    INotificationPublisher IFacadeContainer<INotificationPublisher>.Facade => _notificationPublisherFacade;
}
