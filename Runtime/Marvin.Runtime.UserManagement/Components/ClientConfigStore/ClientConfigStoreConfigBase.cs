using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.UserManagement.ClientConfigStore
{
    /// <summary>
    /// Base config for the client config store
    /// </summary>
    [DataContract]
    public abstract class ClientConfigStoreConfigBase : UpdatableEntry, IPluginConfig
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public abstract string PluginName { get; }
    }
}