using System;
using System.Collections.Generic;
using Marvin.Modules;

namespace Marvin.Runtime.UserManagement.UserAuthenticator
{
    /// <summary>
    /// Interface for the user authenticator.
    /// </summary>
    public interface IUserAuthenticator : IConfiguredPlugin<UserAuthenticatorConfigBase>
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
