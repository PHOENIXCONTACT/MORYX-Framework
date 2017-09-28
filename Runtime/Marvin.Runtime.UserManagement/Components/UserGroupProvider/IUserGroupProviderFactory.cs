using Marvin.Container;

namespace Marvin.Runtime.UserManagement.UserGroupProvider
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IUserGroupProviderFactory
    {
        /// <summary>
        /// Create authenticator with this name
        /// </summary>
        IUserGroupProvider Create(UserGroupProviderConfigBase config);

        /// <summary>
        /// Destroy authenticator instance
        /// </summary>
        void Destroy(IUserGroupProvider instance);
    }
}
