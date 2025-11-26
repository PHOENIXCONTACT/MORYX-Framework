// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Communication;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Notifications;
using Moryx.Runtime.Modules;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// The main controller of all resource modules.
    /// </summary>
    public class ModuleController : ServerModuleBase<ModuleConfig>,
        IFacadeContainer<IResourceManagement>,
        IFacadeContainer<IResourceTypeTree>,
        IFacadeContainer<INotificationSource>
    {
        internal const string ModuleName = "ResourceManager";

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        /// <summary>
        /// Generic component to access every data model
        /// </summary>
        public IDbContextManager DbContextManager { get; }

        /// <inheritdoc />
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager contextManager)
            : base(containerFactory, configManager, loggerFactory)
        {
            DbContextManager = contextManager;
        }

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override Task OnInitializeAsync()
        {
            // Extend container
            Container.RegisterNotifications();
            Container.ActivateDbContexts(DbContextManager);

            // Register imports
            Container
                .SetInstance((IConfigManager)ConfigManager);

            // Register for communication
            Container.Register<IBinaryConnectionFactory>();
            Container.LoadComponents<IBinaryConnection>();

            // Load initializers
            Container.LoadComponents<IResourceInitializer>();

            // Load resources
            Container.LoadComponents<IResource>();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override Task OnStartAsync()
        {
            // Start type controller for resource and proxy creation
            Container.Resolve<IResourceTypeController>().Start();

            // Load manager to boot resources
            var resourceManager = Container.Resolve<IResourceManager>();
            resourceManager.Initialize();

            // Boot up manager
            resourceManager.Start();

            // Activate external facade to register events
            ActivateFacade(_resourceTypeTreeFacade);
            ActivateFacade(_notificationSourceFacade);
            ActivateFacade(_resourceManagementFacade);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override Task OnStopAsync()
        {
            // Tear down facades
            DeactivateFacade(_notificationSourceFacade);
            DeactivateFacade(_resourceManagementFacade);
            DeactivateFacade(_resourceTypeTreeFacade);
            var resourceManager = Container.Resolve<IResourceManager>();
            resourceManager.Stop();
            return Task.CompletedTask;
        }

        private readonly ResourceManagementFacade _resourceManagementFacade = new();
        private readonly ResourceTypeTreeFacade _resourceTypeTreeFacade = new();
        IResourceManagement IFacadeContainer<IResourceManagement>.Facade => _resourceManagementFacade;

        IResourceTypeTree IFacadeContainer<IResourceTypeTree>.Facade => _resourceTypeTreeFacade;

        private readonly NotificationSourceFacade _notificationSourceFacade = new(ModuleName);
        INotificationSource IFacadeContainer<INotificationSource>.Facade => _notificationSourceFacade;
    }
}
