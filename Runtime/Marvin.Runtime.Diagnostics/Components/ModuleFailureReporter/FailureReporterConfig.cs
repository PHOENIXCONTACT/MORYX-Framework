using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Marvin.Runtime.Diagnostics.ModuleFailureReporter
{
    /// <summary>
    /// Configuration for the failure reporter.
    /// </summary>
    [DataContract]
    public class FailureReporterConfig : DiagnosticsPluginConfigBase
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public override string PluginName { get { return FailureReporterPlugin.PluginName; } set { } }

        /// <summary>
        /// Configuration of the mailing client.
        /// </summary>
        [DataMember]
        public MailClientConfig MailClient { get; set; }

        /// <summary>
        /// Interval for the retry of sending the mail when an error occured.
        /// </summary>
        [DataMember]
        [DefaultValue(360000)]
        public int RetryIntervalMs { get; set; }

        /// <summary>
        /// Flag for include the active buttons.
        /// </summary>
        [DataMember]
        public bool IncludeActiveButtons { get; set; }

        /// <summary>
        /// List of targets where to send the report.
        /// </summary>
        [DataMember]
        public List<FailureReportTarget> ReportTargets { get; set; }
    }

    /// <summary>
    /// configuration of a failure report target.
    /// </summary>
    [DataContract]
    public class FailureReportTarget
    {
        /// <summary>
        /// E-mail address of the target.
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// Name of the target.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Filter to set the interest of a plugin failure.
        /// </summary>
        [DataMember]
        [DefaultValue(PluginFailureFilter.All)]
        public PluginFailureFilter PluginFailureFilter { get; set; }
        /// <summary>
        /// Name of the plugins for in which the target is interested in.
        /// </summary>
        [DataMember]
        public string PluginNames { get; set; }

        /// <summary>
        /// Format: "Address": "PluginFailureFilter".
        /// </summary>
        /// <returns>Format: "Address": "PluginFailureFilter".</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", Address, PluginFailureFilter);
        }
    }

    /// <summary>
    /// Filter to set the interest of a plugin failure.
    /// </summary>
    public enum PluginFailureFilter
    {
        /// <summary>
        /// Get all failure reports.
        /// </summary>
        All,
        /// <summary>
        /// Get only reports for modules with database access.
        /// </summary>
        DataAccess,
        /// <summary>
        /// Get only reports for plugins with configured names.
        /// </summary>
        Named
    }
}
