using System.Collections.Generic;
using System.Security.Principal;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.UserManagement.UserGroupProvider
{
    /// <summary>
    /// Interface for user groups.
    /// </summary>
    public interface IUserGroupProvider : IConfiguredPlugin<UserGroupProviderConfigBase>
    {
        /// <summary>
        /// Get users groups
        /// </summary>
        IEnumerable<string> GetUserGroups(WindowsIdentity user);
    }
}
