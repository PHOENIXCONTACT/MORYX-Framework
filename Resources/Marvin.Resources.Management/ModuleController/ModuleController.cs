using Marvin.AbstractionLayer.Resources;
using Marvin.Communication;
using Marvin.Container;
using Marvin.Model;
using Marvin.Modules.Server;
using Marvin.Notifications;
using Marvin.Resources.Model;
using Marvin.Runtime.Base;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Container;
using Marvin.Runtime.ServerModules;
using Marvin.Tools.Wcf;

namespace Marvin.Resources.Management
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
            Container.SetInstances(ResourceModel, WcfClientFactory, ConfManager);

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
            // TODO: Add the event to activate the notification facade. Use this until Platform 3.0 has a better solution for this problem.
            ((IServerModule)this).State.Changed += OnModuleStateChanged;

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

            // TODO: Remove the event to deactivate the notification facade. Use this until Platform 3.0 has a better solution for this problem.
            ((IServerModule)this).State.Changed -= OnModuleStateChanged;
        }

        /// TODO: This is a quick fix to only start the notification facade when the module itself is in the state of running and should be changed with Platform 3.0
        private void OnModuleStateChanged(object sender, ModuleStateChangedEventArgs e)
        {
            if (e.NewState == ServerModuleState.Running && !_notificationSourceFacade.IsActivated)
				_notificationSourceFacade.RaiseActivated();
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