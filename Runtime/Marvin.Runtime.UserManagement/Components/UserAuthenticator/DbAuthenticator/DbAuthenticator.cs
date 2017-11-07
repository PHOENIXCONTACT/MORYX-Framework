using System.Collections.Generic;
using System.Linq;
using Marvin.Container;
using Marvin.Model;
using Marvin.Modules.ModulePlugins;
using Marvin.Runtime.UserManagement.Model;

namespace Marvin.Runtime.UserManagement.UserAuthenticator
{
    /// <summary>
    /// Provide access to the authorization configs of a database.
    /// </summary>
    [ExpectedConfig(typeof(DbAuthenticatorConfig))]
    [Plugin(LifeCycle.Transient, typeof(IUserAuthenticator), Name = ComponentName)]
    public class DbAuthenticator : IUserAuthenticator
    {
        internal const string ComponentName = "DbAuthenticator";

        /// <summary>
        /// Model resolver. Injected by castle.
        /// </summary>
        public IModelResolver ModelResolver { get; set; }

        private DbAuthenticatorConfig _config;
        private IUnitOfWorkFactory _factory;

        /// <inheritdoc />
        public void Initialize(UserAuthenticatorConfigBase config)
        {
            _config = (DbAuthenticatorConfig)config;
            _factory = ModelResolver.GetByNamespace(UserManagementConstants.Namespace);
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
        /// Fetch the appllication configuration for the user groups.
        /// </summary>
        /// <param name="userGroups">Groups in which the user is in.</param>
        /// <returns>Applicaton configuration for the requested user groups.</returns>
        public ApplicationConfiguration GetPluginConfiguration(string[] userGroups)
        {
            //Get user-Info
            var config = new ApplicationConfiguration { Name = _config.ApplicationName };

            using (var unit = _factory.Create())
            {
                var appRepo = unit.GetRepository<IApplicationRepository>();
                var application = appRepo.GetByApplicationName(_config.ApplicationName);
                if (application == null)
                    return config;

                //first build shell and shell-configuration
                config.Shell = new ModuleConfiguration
                {
                    Application = application.ApplicationName,
                    Enabled = true,
                    Library = application.Shell.LibraryName,
                    Dependencies = application.Shell.Dependencies.Select(d => d.LibraryName).ToList()
                };

                // Now get plugins for the application, for which user has the required permissions
                foreach (var plugin in application.AvailablePlugins)
                {
                    // User gets the plugin, IF there is ANY AccessPermission for a user group which the user is a member of  
                    if (plugin.AccessPermissions.Any(accessPermission => userGroups.Contains(accessPermission.UserGroup.GroupName)))
                    {
                        config.Modules.Add(new ModuleConfiguration
                        {
                            Application = _config.ApplicationName,
                            Enabled = true,
                            Library = plugin.LibraryName,
                            SortIndex = plugin.SortIndex,
                            Dependencies = plugin.Dependencies.Select(d => d.LibraryName).ToList()
                        });
                    }
                }

            }
            return config;
        }
        /// <summary>
        /// Fetch the operation access of a module for the groups the user is in.
        /// </summary>
        /// <param name="module">The module for which the operation access should be fetched.</param>
        /// <param name="userGroups">Groups in which the user is in.</param>
        /// <returns>The operation accesses.</returns>
        public Dictionary<string, OperationAccess> GetOperationAccesses(ModuleConfiguration module, string[] userGroups)
        {
            using (var uow = _factory.Create())
            {
                // Get related library and access link
                var lib = uow.GetRepository<ILibraryRepository>().GetByLibraryName(module.Library);
                var libAccesses = lib.AccessPermissions.Where(access => userGroups.Contains(access.UserGroup.GroupName));

                var opDict = new Dictionary<string, OperationAccess>();
                foreach (var operation in libAccesses.SelectMany(access => GroupOperations(access.OperationGroups)))
                {
                    if (!opDict.ContainsKey(operation.Key) || operation.Value > opDict[operation.Key])
                        opDict[operation.Key] = operation.Value;
                }

                return opDict;
            }

        }

        private IEnumerable<KeyValuePair<string, OperationAccess>> GroupOperations(IEnumerable<OperationGroup> groups)
        {
            // Read all operations from first level groups
            IEnumerable<KeyValuePair<string, OperationAccess>> dict = new List<KeyValuePair<string, OperationAccess>>();
            dict = groups.Aggregate(dict, (current, operationGroup) => current
                // Union current collection with transformation of all operations
                                    .Union(operationGroup.OperationGroupLinks.Select(Convert))
                // Union with all operations from embedded groups
                                    .Union(GroupOperations(operationGroup.EmbeddedGroups)));
            return dict;
        }

        private KeyValuePair<string, OperationAccess> Convert(OperationGroupLink link)
        {
            return new KeyValuePair<string, OperationAccess>(link.Operation.Name, (OperationAccess)(int)link.AccessType);
        }
    }
}
