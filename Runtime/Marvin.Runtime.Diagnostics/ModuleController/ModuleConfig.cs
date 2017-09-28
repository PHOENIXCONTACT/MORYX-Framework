using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Runtime.Configuration;


namespace Marvin.Runtime.Diagnostics
{
    /// <summary>
    /// Configuration for diagnostics.
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// List of configured diagnositcs modules.
        /// </summary>
        [DataMember]
        [Description("List of configured diagnositcs modules.")]
        [PluginConfigs(typeof(IDiagnosticsPlugin), false)]
        public List<DiagnosticsPluginConfigBase> Plugins { get; set; }
    }
}
