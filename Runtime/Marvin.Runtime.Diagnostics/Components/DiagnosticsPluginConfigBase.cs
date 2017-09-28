using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.Diagnostics
{
    /// <summary>
    /// configuration for a diagnostic plugin.
    /// </summary>
    [DataContract]
    public class DiagnosticsPluginConfigBase : UpdatableEntry, IPluginConfig
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        [DataMember]
        public virtual string PluginName { get; set; }

        /// <summary>
        /// Should this plugin be activated?
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Format is: "PluginName: "(not)" active
        /// </summary>
        /// <returns>Formated string like in description.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}active", PluginName, IsActive ? string.Empty : "not ");
        }
    }
}
