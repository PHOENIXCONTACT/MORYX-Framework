using System.Collections.Generic;
using System.Linq;
using Marvin.Container;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.UserManagement.UserAuthenticator
{
    /// <summary>
    /// Provide access to the authentication configs.
    /// </summary>
    [ExpectedConfig(typeof(ConfigAuthorizationConfig))]
    [Plugin(LifeCycle.Transient, typeof(IUserAuthenticator), Name = ComponentName)]
    public class ConfigAuthenticator : IUserAuthenticator
    {
        internal const string ComponentName = "ConfigAuthorization";
        /// <summary>
        /// Common group.
        /// </summary>
        public const string CommonGroup = "All";

        private ConfigAuthorizationConfig _config;

        /// <summary>
        /// Initialize the config authenticator with the given config.
        /// </summary>
        /// <param name="config">an UserAuthenticatorConfig.</param>
        public void Initialize(UserAuthenticatorConfigBase config)
        {
            _config = (ConfigAuthorizationConfig) config;
        }

        /// <summary>
        /// Dispose the ConfigAuthenticator.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Start the ConfigAuthenticator. 
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Fetch the appllication configuration for the user groups.
        /// </summary>
        /// <param name="userGroups">Groups in which the user is in.</param>
        /// <returns>Applicaton configuration for the requested user groups.</returns>
        public ApplicationConfiguration GetPluginConfiguration(string[] userGroups)
        {
            var accesConfig = GetMatchingConfig(userGroups);
            return accesConfig == null ? null : ConvertAppConfig(accesConfig.Configuration);
        }

        /// <summary>
        /// Fetch the operation access of a module for the groups the user is in.
        /// </summary>
        /// <param name="module">The module for which the operation access should be fetched.</param>
        /// <param name="userGroups">Groups in which the user is in.</param>
        /// <returns>The operation accesses.</returns>
        public Dictionary<string, OperationAccess> GetOperationAccesses(ModuleConfiguration module, string[] userGroups)
        {
            var accesConfig = GetMatchingConfig(userGroups);
            return accesConfig == null ? null : accesConfig.OperationAccesses.ToDictionary(access => access.Operation, access => access.AccessRight);
        }

        private AccessConfiguration GetMatchingConfig(IEnumerable<string> userGroups)
        {
            var accesConfig = _config.AccessConfigurations.FirstOrDefault(conf => userGroups.Contains(conf.UserGroup))
                           ?? _config.AccessConfigurations.FirstOrDefault(conf => conf.UserGroup == CommonGroup);
            return accesConfig;
        }

        private ApplicationConfiguration ConvertAppConfig(ApplicationConfig config)
        {
            var converted = new ApplicationConfiguration
            {
                Name = config.Application,
                Shell = ConvertPluginConfig(config.ShellDll),
                Modules = config.PluginConfigs.Select(ConvertPluginConfig).ToList()
            };
            return converted;
        }

        private ModuleConfiguration ConvertPluginConfig(PluginConfig config)
        {
            var converted = new ModuleConfiguration
            {
                Application = config.Application,
                SortIndex = config.SortIndex,
                Enabled = true,
                Library = config.PluginDll,
                OverriddenDisplayName = config.OverrideDisplayName,
                Dependencies = config.Dependencies.Select(dep => dep.LibraryName).ToList()
            };
            return converted;
        }
    }
}
