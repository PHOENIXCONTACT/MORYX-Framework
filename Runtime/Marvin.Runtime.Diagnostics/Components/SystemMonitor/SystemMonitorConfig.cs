using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.Diagnostics.SystemMonitor
{
    /// <summary>
    /// Configuration for the system monitor.
    /// </summary>
    [DataContract]
    public class SystemMonitorConfig : DiagnosticsPluginConfigBase
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public override string PluginName { get { return SystemMonitorPlugin.PluginName; } }

        /// <summary>
        /// Path where the output will land.
        /// </summary>
        [DataMember]
        public string OutputPath { get; set; }

        /// <summary>
        /// The invervall in which the class will monitor the system.
        /// </summary>
        [DataMember]
        [DefaultValue(800)]
        [IntegerSteps(100, 6500, 2, StepMode.Multiplication)]
        public int MonitorIntervalMs { get; set; }

        /// <summary>
        /// Flag to enable the output written to a file configurated in <see cref="OutputPath"/>.
        /// </summary>
        [DataMember]
        public bool EnableFileOutput { get; set; }
    }
}
