// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Communication;
using Moryx.Container;
using Moryx.Model;
using Moryx.Notifications;
using Moryx.Resources.Model;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.Tools.Wcf;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// The main controller of all resource modules. 
    /// </summary>
    [ServerModule(ModuleName)]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, 
        IFacadeContainer<IResourceManagement>, 
        IFacadeContainer<INotificationSource>
    {
        internal const string ModuleName = "ResourceManager";

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        /// <summary>Injected property</summary>
        [Named(ResourcesConstants.Namespace)]
        public IUnitOfWorkFactory ResourceModel { get; set; }

        /// <summary>Injected property</summary>
        public IWcfClientFactory WcfClientFactory { get; set; }

        /// <summary>Injected property</summary>
        public IRuntimeConfigManager ConfManager { get; set; }

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
            // Register imports
            Container.RegisterNotifications();
            Container.SetInstance(ResourceModel).SetInstance(WcfClientFactory).SetInstance(ConfManager);

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
            
            var resourceManager = Container.Resolve<IResourceManager>();
            resourceManager.Stop();
        }

        #endregion

        #region FacadeContainer

        private readonly ResourceManagementFacade _resourceManagementFacade = new ResourceManagementFacade();
        IResourceManagement IFacadeContainer<IResourceManagement>.Facade => _resourceManagementFacade;

        private readonly NotificationSourceFacade _notificationSourceFacade = new NotificationSourceFacade(ModuleName);
        INotificationSource IFacadeContainer<INotificationSource>.Facade => _notificationSourceFacade;

        #endregion
    }
}
