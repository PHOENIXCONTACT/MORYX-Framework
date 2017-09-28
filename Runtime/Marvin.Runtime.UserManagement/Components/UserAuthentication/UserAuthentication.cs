using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Runtime.UserManagement.UserAuthenticator;
using Marvin.Runtime.UserManagement.UserGroupProvider;

namespace Marvin.Runtime.UserManagement.UserAuthentication
{
    [Plugin(LifeCycle.Singleton, typeof(IUserAuthentication))]
    internal class UserAuthentication : IUserAuthentication, ILoggingComponent
    {
        #region dependency injection

        // Injected by Castle
        public IModuleLogger Logger { get; set; }
        public IUserAuthenticatorFactory UserAuthenticatorFactory { get; set; }
        public IUserGroupProviderFactory UserGroupProviderFactory { get; set; }
        public ModuleConfig Config { get; set; }

        #endregion

        #region Fields

        private readonly Dictionary<string, IUserAuthenticator> _authenticators = new Dictionary<string, IUserAuthenticator>();
        private readonly List<IUserGroupProvider> _providers = new List<IUserGroupProvider>();

        #endregion

        #region IModulePlugin

        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        public void Start()
        {
            foreach (var applicationAccess in Config.AuthenticatorConfigs)
            {
                _authenticators[applicationAccess.ApplicationName] = UserAuthenticatorFactory.Create(applicationAccess);
            }

            foreach (var groupProvider in Config.UserGroupProviders.Where(gp => gp.Active))
            {
                _providers.Add(UserGroupProviderFactory.Create(groupProvider));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _authenticators.Clear();
            _providers.Clear();
        }

        #endregion

        #region IUserAuthentication

        /// <summary>
        /// Gets the user infos.
        /// </summary>
        public UserModel GetUserInfos()
        {
            return GetUserInfos(ServiceSecurityContext.Current.WindowsIdentity);
        }

        /// <summary>
        /// Get infos for this user
        /// </summary>
        public UserModel GetUserInfos(WindowsIdentity identity)
        {
            var myUser = new UserModel {Username = identity.Name, Groups = GetUserGroups(identity)};
            return myUser;
        }

        /// <summary>
        /// Get an application configuration from an application name.
        /// </summary>
        /// <param name="application">The application name for which the configuration should be fetched.</param>
        /// <returns>delivers an <see cref="ApplicationConfiguration"/>.</returns>
        public ApplicationConfiguration GetApplication(string application)
        {
            var user = ServiceSecurityContext.Current.WindowsIdentity;
            if (_authenticators.ContainsKey(application))
                return _authenticators[application].GetPluginConfiguration(GetUserGroups(user).ToArray());

            var config = new ApplicationConfiguration {Name = application};
            return config;
        }

        /// <summary>
        /// Get the operation access for specified module configuration.
        /// </summary>
        /// <param name="module">The module configuration for which the operation access should be fetched.</param>
        /// <returns>
        /// Dictionary with the operation access of the module.
        /// </returns>
        public Dictionary<string, OperationAccess> GetOperationAccesses(ModuleConfiguration module)
        {
            var user = ServiceSecurityContext.Current.WindowsIdentity;
            if (_authenticators.ContainsKey(module.Application))
                return _authenticators[module.Application].GetOperationAccesses(module, GetUserGroups(user).ToArray());

            return new Dictionary<string, OperationAccess>();
        }

        private List<string> GetUserGroups(WindowsIdentity user)
        {
            var allGroups = new List<string>();
            foreach (var provider in _providers)
            {
                allGroups.AddRange(provider.GetUserGroups(user).Except(allGroups));
            }
            return allGroups;
        }

        #endregion
    }
}
