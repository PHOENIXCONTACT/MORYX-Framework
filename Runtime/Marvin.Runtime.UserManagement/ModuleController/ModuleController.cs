using Marvin.Model;
using Marvin.Runtime.Base;
using Marvin.Runtime.Container;
using Marvin.Runtime.ServerModules;
using Marvin.Runtime.UserManagement.ClientConfigStore;
using Marvin.Runtime.UserManagement.FirstStarter;
using Marvin.Runtime.UserManagement.UserAuthentication;
using Marvin.Runtime.UserManagement.UserAuthenticator;
using Marvin.Runtime.UserManagement.UserGroupProvider;
using Marvin.Runtime.UserManagement.Wcf;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.UserManagement
{
    /// <summary>
    /// User manager module.
    /// </summary>
    [ServerModule(ModuleName)]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, IFacadeContainer<IUserManager>
    {
        /// <summary>
        /// Model resolver injected by castle.
        /// </summary>
        public IModelResolver ModelResolver { get; set; }

        /// <summary>
        /// Host of the authorization service
        /// </summary>
        private IConfiguredServiceHost _host;

        /// <summary>
        /// Name of this module
        /// </summary>
        private const string ModuleName = "UserManager";

        /// <summary>
        /// private field for the current client config store
        /// </summary>
        private IClientConfigStore _currentClientConfigStore;

        /// <summary>
        /// factory for the client config storage
        /// </summary>
        private IClientConfigStoreFactory _currentConfigStoreFactory;

        /// <summary>
        /// Get the name of this module.
        /// </summary>
        public override string Name
        {
            get { return ModuleName; }
        }

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
            Container.SetInstance(ModelResolver);
            Container.LoadComponents<IUserAuthenticator>();
            Container.LoadComponents<IUserGroupProvider>();
            Container.LoadComponents<IUserManagementFirstStarter>();

            _currentConfigStoreFactory = Container.Resolve<IClientConfigStoreFactory>();
            _currentClientConfigStore = _currentConfigStoreFactory.CreateStore(Config.ConfigStorage);

        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
        {
            var starter = Container.Resolve<IUserManagementFirstStarter>();
            if (!Config.ConfigInitialized)
                starter.InitializeUserManagement(Config, ConfigManager);

            Container.Resolve<IUserAuthentication>().Start();
            _currentClientConfigStore.Start();

            _host = Container.Resolve<IConfiguredHostFactory>().CreateHost<IUserManagementService>(Config.HostConfig);
            _host.Start();

            ActivateFacade(_facade);
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
            DeactivateFacade(_facade);

            _host.Dispose();
            _host = null;

            if (_currentClientConfigStore != null)
            {
                _currentConfigStoreFactory.Destroy(_currentClientConfigStore);
            }
        }
        #endregion

        private readonly UserManagerFacade _facade = new UserManagerFacade();

        /// <summary>
        /// Provides the facade.
        /// </summary>
        public IUserManager Facade
        {
            get { return _facade; }
        }
    }
}