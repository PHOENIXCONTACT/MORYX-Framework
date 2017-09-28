using System.Collections.Generic;
using System.Net;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Runtime.UserManagement.UserAuthenticator;
using Marvin.Runtime.UserManagement.UserGroupProvider;

namespace Marvin.Runtime.UserManagement.FirstStarter
{
    /// <summary>
    /// Enables the initialization of the user management because after deployment some 
    /// possible configuration front ends might depend on a running user management.
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IUserManagementFirstStarter))]
    public class MaintenanceFirstStarter : IUserManagementFirstStarter
    {
        private const string AppName = "Maintenance";

        private int _index = -1;
        private readonly List<Dependency> _sharedDep = new List<Dependency>();

        /// <summary>
        /// Initialize the user management with the given config.
        /// </summary>
        /// <param name="config">configuration for the user managemengt.</param>
        /// <param name="configManager">The configuration manager.</param>
        public void InitializeUserManagement(ModuleConfig config, IConfigManager configManager)
        {
            var authenticatorConfig = new ConfigAuthorizationConfig
            {
                ApplicationName = AppName,
                AccessConfigurations = new List<AccessConfiguration>()
            };

            // Create default access for the maintenance page
            var defaultMaintenance = CreateAdminAccess();
            defaultMaintenance.UserGroup = ConfigAuthenticator.CommonGroup;
            authenticatorConfig.AccessConfigurations.Add(defaultMaintenance);

            // Create admin access for the maintenance page
            var maintenance = CreateAdminAccess();
            maintenance.UserGroup = Dns.GetHostName() + "\\Marvin_Admin";
            authenticatorConfig.AccessConfigurations.Add(maintenance);

            config.AuthenticatorConfigs.Add(authenticatorConfig);

            // Add LDAP group provider
            config.UserGroupProviders.Add(new UserGroupProviderConfigBase { PluginName = UserGroupProviderConfigBase.DefaultProvider, Active = true });

            config.ConfigInitialized = true;

            configManager.SaveConfiguration(config);
        }

        private AccessConfiguration CreateAdminAccess()
        {
            _index = -1;

            var maintenance = new AccessConfiguration { OperationAccesses = new List<OperationAccessPair>() };
            var pluginsConf = new ApplicationConfig
            {
                Application = AppName,
                ShellDll = Create("Marvin.Maintenance.Gui.DefaultShell.dll")
            };
            maintenance.Configuration = pluginsConf;

            // Configure plugins tab
            var pluginConf = Create("Marvin.Maintenance.Gui.Plugin.Module.dll");
            pluginsConf.PluginConfigs.Add(pluginConf);

            // Configure data model tab
            pluginConf = Create("Marvin.Maintenance.Gui.Plugin.DbConfig.dll");
            pluginsConf.PluginConfigs.Add(pluginConf);

            // Configure logging tab
            pluginConf = Create("Marvin.Maintenance.Gui.Plugin.Logging.dll");
            pluginsConf.PluginConfigs.Add(pluginConf);

            // Configure data store tab
            pluginConf = Create("Silverlight.Shared.Gui.Plugin.DataStore.dll");
            pluginConf.OverrideDisplayName = "Datastore";
            pluginsConf.PluginConfigs.Add(pluginConf);

            // Define full plugin access
            maintenance.OperationAccesses.Add(new OperationAccessPair { Operation = "PluginAccess", AccessRight = OperationAccess.Full });
            maintenance.OperationAccesses.Add(new OperationAccessPair { Operation = "ConfigAccess", AccessRight = OperationAccess.Full });

            return maintenance;
        }

        private PluginConfig Create(string name)
        {
            var pluginConf = new PluginConfig
            {
                Application = AppName,
                PluginDll = name,
                SortIndex = _index++,
                Dependencies = _sharedDep
            };
            return pluginConf;
        }
    }
}
