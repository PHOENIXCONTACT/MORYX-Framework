using System;
using System.Collections.Generic;
using System.Security.Principal;
using Marvin.Container;
using Marvin.Logging;

namespace Marvin.Runtime.UserManagement.UserGroupProvider
{
    [Plugin(LifeCycle.Singleton, typeof(IUserGroupProvider), Name = UserGroupProviderConfigBase.DefaultProvider)]
    internal class LdapGroupProvider : IUserGroupProvider, ILoggingComponent
    {
        private UserGroupProviderConfigBase _config;

        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        public void Initialize(UserGroupProviderConfigBase config)
        {
            _config = config;
        }

        /// <summary>
        /// Get users groups
        /// </summary>
        public IEnumerable<string> GetUserGroups(WindowsIdentity user)
        {          
            var groups = new List<string>();
            if (user.Groups == null)
                return groups.ToArray();

            foreach (var group in user.Groups)
            {
                try
                {
                    var newGroup = @group.Translate(typeof(NTAccount)).ToString();
                    groups.Add(newGroup);
                }
                catch (Exception ex)
                {
                    Logger.LogException(LogLevel.Warning, ex, "Failed to translate user group {0}", @group);
                }
            }
            return groups.ToArray();
        }

        public IModuleLogger Logger { get; set; }
    }
}
