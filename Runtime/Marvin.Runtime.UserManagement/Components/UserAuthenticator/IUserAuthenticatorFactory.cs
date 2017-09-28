using Marvin.Container;

namespace Marvin.Runtime.UserManagement.UserAuthenticator
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IUserAuthenticatorFactory
    {
        /// <summary>
        /// Create authenticator with this name
        /// </summary>
        IUserAuthenticator Create(UserAuthenticatorConfigBase config);

        /// <summary>
        /// Destroy authenticator instance
        /// </summary>
        void Destroy(IUserAuthenticator instance);
    }
}
