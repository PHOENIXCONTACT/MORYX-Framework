using System.Runtime.Serialization;

namespace Marvin.Runtime.UserManagement.UserAuthenticator
{
    /// <summary>
    /// Configuration for the database authorization.
    /// </summary>
    [DataContract]
    public class DbAuthenticatorConfig : UserAuthenticatorConfigBase
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public override string PluginName
        {
            get { return DbAuthenticator.ComponentName; }
        }

        /// <summary>
        /// The type how to store the authentications.
        /// </summary>
        public override AuthenticationStore AuthenticationStoreType
        {
            get { return UserAuthenticator.AuthenticationStore.Database; }
        }

        /// <summary>
        /// Name of the authenticationstore.
        /// </summary>
        public override string AuthenticationStore
        {
            get { return null; }
        }
    }
}
