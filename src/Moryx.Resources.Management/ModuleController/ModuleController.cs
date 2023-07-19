// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Communication;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Notifications;
using Moryx.Resources.Management.Facades;
using Moryx.Runtime.Modules;
using System;

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

        public ModuleController(IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager contextManager) 
            : base(configManager, loggerFactory)
        {
            DbContextManager = contextManager;
        }

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize(IServiceCollection services)
        {
            // Extend container
            services.RegisterNotifications();
            services.ActivateDbContexts(DbContextManager);

            // Register imports
            services
                .AddSingleton(ConfigManager);

            // Register for communication
            services.AddFactory<IBinaryConnectionFactory>();
            services.AddFromAppDomain<IBinaryConnection>();

            // Load initializers
            services.AddFromAppDomain<IResourceInitializer>();

            // Load resources
            services.AddFromAppDomain<IResource>();
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart(IServiceProvider serviceProvider)
        {
            // Start type controller for resource and proxy creation
            serviceProvider.GetRequiredService<IResourceTypeController>().Start();

            // Load manager to boot resources
            var resourceManager = serviceProvider.GetRequiredService<IResourceManager>();
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
        protected override void OnStop(IServiceProvider serviceProvider)
        {
            // Tear down facades
            DeactivateFacade(_notificationSourceFacade);
            DeactivateFacade(_resourceManagementFacade);
            DeactivateFacade(_resourceTypeTreeFacade);

            var resourceManager = serviceProvider.GetRequiredService<IResourceManager>();
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
