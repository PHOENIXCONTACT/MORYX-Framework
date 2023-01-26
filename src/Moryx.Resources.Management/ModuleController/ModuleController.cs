// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Communication;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Notifications;
using Moryx.Resources.Management.Facades;
using Moryx.Runtime.Modules;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// The main controller of all resource modules.
    /// </summary>
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>,
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

        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager contextManager) 
            : base(containerFactory, configManager, loggerFactory)
        {
            DbContextManager = contextManager;
        }

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
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
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
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
  
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
            // Tear down facades
            DeactivateFacade(_notificationSourceFacade);
            DeactivateFacade(_resourceManagementFacade);
            DeactivateFacade(_resourceTypeTreeFacade);
            var resourceManager = Container.Resolve<IResourceManager>();
            resourceManager.Stop();
        }

        private readonly ResourceManagementFacade _resourceManagementFacade = new ResourceManagementFacade();
        private readonly ResourceTypeTreeFacade _resourceTypeTreeFacade = new ResourceTypeTreeFacade();
        IResourceManagement IFacadeContainer<IResourceManagement>.Facade => _resourceManagementFacade;

        IResourceTypeTree IFacadeContainer<IResourceTypeTree>.Facade => _resourceTypeTreeFacade;

        private readonly NotificationSourceFacade _notificationSourceFacade = new NotificationSourceFacade(ModuleName);
        INotificationSource IFacadeContainer<INotificationSource>.Facade => _notificationSourceFacade;
    }
}
