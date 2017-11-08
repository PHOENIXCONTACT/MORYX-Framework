using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Modules;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Contracts
{
    /// <summary>
    /// Base configuration for a maintenance plugin.
    /// </summary>
    [DataContract]
    public abstract class MaintenancePluginConfig : UpdatableEntry, IPluginConfig
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public abstract string PluginName { get; }

        /// <summary>
        /// Endpoint which the plugin provides.
        /// </summary>
        [DataMember]
        public HostConfig ProvidedEndpoint { get; set; }

        /// <summary>
        /// Flag to activate or deactivate the plugin.
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }
    }
}
