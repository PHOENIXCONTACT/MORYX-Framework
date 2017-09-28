using System.Collections.Generic;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.UserManagement.UserAuthenticator
{
    /// <summary>
    /// Interface for the user authenticator.
    /// </summary>
    public interface IUserAuthenticator : IConfiguredModulePlugin<UserAuthenticatorConfigBase>
    {
        /// <summary>
        /// Get the plugin config for this user
        /// </summary>
        ApplicationConfiguration GetPluginConfiguration(string[] userGroups);

        /// <summary>
        /// Get operation access for this plugin
        /// </summary>
        Dictionary<string, OperationAccess> GetOperationAccesses(ModuleConfiguration module, string[] userGroups);
    }
}
