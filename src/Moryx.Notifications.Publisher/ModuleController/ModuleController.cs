// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Runtime.Modules;
using System.ComponentModel;

namespace Moryx.Notifications.Publisher
{
    /// <summary>
    /// Module controller of the notifications module
    /// </summary>
    [Description("Module to handle publishing of notifications from the applications.")]
    public class ModuleController : ServerModuleBase<ModuleConfig>,
        IFacadeContainer<INotificationPublisher>
    {
        private const string ModuleName = "NotificationPublisher";

        /// <inheritdoc />
        public override string Name => ModuleName;

        /// <summary>
        /// Manager for creating configured DbContext instances
        /// </summary>
        public IDbContextManager DbContextManager { get; }

        /// <summary>
        /// Notification facades from other modules
        /// </summary>
        [RequiredModuleApi]
        public INotificationSource[] NotificationSources { get; set; }

        /// <summary>
        /// Creates a new instance of the module
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager contextManager)
            : base(containerFactory, configManager, loggerFactory)
        {
            DbContextManager = contextManager;
        }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            // Register global components
            Container.ActivateDbContexts(DbContextManager);

            // Load additional processors
            Container.LoadComponents<INotificationProcessor>();

            // Register sources
            foreach (var source in NotificationSources)
                Container.SetInstance(source, source.Name);

            // Initialize components
            Container.Resolve<INotificationManager>().Initialize();
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            Container.Resolve<INotificationManager>().Start();

            ActivateFacade(_notificationPublisherFacade);
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            DeactivateFacade(_notificationPublisherFacade);

            Container.Resolve<INotificationManager>().Stop();
        }

        private readonly NotificationPublisherFacade _notificationPublisherFacade = new();

        INotificationPublisher IFacadeContainer<INotificationPublisher>.Facade => _notificationPublisherFacade;
    }
}

