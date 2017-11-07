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

        public void Initialize(UserGroupProviderConfigBase config)
        {
            _config = config;
        }

        public void Start()
        {
        }

        public void Stop()
        {
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
