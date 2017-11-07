using System.Collections.Generic;
using System.Security.Principal;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.UserManagement.UserAuthentication
{
    /// <summary>
    /// Central component handling user authentication
    /// </summary>
    internal interface IUserAuthentication : IPlugin
    {
        /// <summary>
        /// Gets the user infos.
        /// </summary>
        UserModel GetUserInfos();

        /// <summary>
        /// Get infos for the given user
        /// </summary>
        /// <returns></returns>
        UserModel GetUserInfos(WindowsIdentity identity);

        /// <summary>
        /// Get an application configuration from an application name.
        /// </summary>
        /// <param name="application">The application name for which the configuration should be fetched.</param>
        /// <returns>delivers an <see cref="ApplicationConfiguration"/>.</returns>
        ApplicationConfiguration GetApplication(string application);

        /// <summary>
        /// Get the operation access for specified module configuration.
        /// </summary>
        /// <param name="module">The module configuration for which the operation access should be fetched.</param>
        /// <returns>Dictionary with the operation access of the module.</returns>
        Dictionary<string, OperationAccess> GetOperationAccesses(ModuleConfiguration module);
    }
}
