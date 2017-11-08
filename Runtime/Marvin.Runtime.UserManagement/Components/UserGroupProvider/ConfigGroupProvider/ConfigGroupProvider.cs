using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Marvin.Container;
using Marvin.Modules;

namespace Marvin.Runtime.UserManagement.UserGroupProvider
{
    /// <summary>
    /// Povides group configuration.
    /// </summary>
    [ExpectedConfig(typeof(ConfigGroupProviderConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IUserGroupProvider), Name = ComponentName)]
    public class ConfigGroupProvider : IUserGroupProvider
    {
        internal const string ComponentName = "ConfigGroupProvider";

        private ConfigGroupProviderConfig _config;

        /// <inheritdoc />
        public void Initialize(UserGroupProviderConfigBase config)
        {
            _config = (ConfigGroupProviderConfig) config;
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
        }

        /// <summary>
        /// Get users groups
        /// </summary>
        /// <param name="user">Groups for this user.</param>
        /// <returns>the groups where the user belongs to.</returns>
        public IEnumerable<string> GetUserGroups(WindowsIdentity user)
        {
            var userModel = _config.Users.FirstOrDefault(u => string.Equals(u.UserName, user.Name, StringComparison.CurrentCultureIgnoreCase));
            return userModel?.Groups.Select(g => g.GroupName) ?? Enumerable.Empty<string>();
        }
    }
}
