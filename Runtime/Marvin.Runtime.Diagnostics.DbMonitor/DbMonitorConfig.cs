using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Runtime.Diagnostics.DbMonitor
{
    /// <summary>
    /// Configuration for the database monitor.
    /// </summary>
    [DataContract]
    public class DbMonitorConfig : DiagnosticsPluginConfigBase
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public override string PluginName { get { return DbMonitorPlugin.PluginName; } set {} }

        /// <summary>
        /// Host were the DbMonitor is running.
        /// </summary>
        [DataMember]
        [CurrentHostName]
        [Description("Host were the DbMonitor is running.")]
        public string HostName { get; set; }

        /// <summary>
        /// Port the DbMonitor is running on.
        /// </summary>
        [DataMember]
        [Description("Port the DbMonitor is running on.")]
        [DefaultValue(1000)]
        public int Port { get; set; }
    }
}
