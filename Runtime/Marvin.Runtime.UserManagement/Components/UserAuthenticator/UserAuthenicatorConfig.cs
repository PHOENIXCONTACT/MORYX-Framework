using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.UserManagement.UserAuthenticator
{
    /// <summary>
    /// Configuration for the user authenticator.
    /// </summary>
    [DataContract]
    public abstract class UserAuthenticatorConfigBase : UpdatableEntry, IPluginConfig
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public abstract string PluginName { get; }

        /// <summary>
        /// The authentication store type. <see cref="UserAuthenticator.AuthenticationStore"/> for details.
        /// </summary>
        public abstract AuthenticationStore AuthenticationStoreType { get; }

        /// <summary>
        /// Gets the authentication store.
        /// </summary>
        public abstract string AuthenticationStore { get; }

        /// <summary>
        /// Name of the application.
        /// </summary>
        [DataMember]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Format is: "Pluginname" - "ApplicationName"
        /// </summary>
        /// <returns>Format is: "Pluginname" - "ApplicationName"</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", PluginName, ApplicationName);
        }
    }

    /// <summary>
    /// Enumeration of the authentication store save settings.
    /// </summary>
    public enum AuthenticationStore
    {
        /// <summary>
        /// Access settings are stored in the authenticator config
        /// </summary>
        Config,
        /// <summary>
        /// Access settings are stored in a file
        /// </summary>
        File,
        /// <summary>
        /// Access settings are stored in a database
        /// </summary>
        Database
    }
}
