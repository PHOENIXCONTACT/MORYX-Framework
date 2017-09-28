using System;
using System.Collections.Generic;

namespace Marvin.Runtime.Diagnostics.ModuleFailureReporter
{
    public partial class MailBody
    {
        /// <summary>
        /// Name of the target of this mail.
        /// </summary>
        public string TargetName { get; set; }
        /// <summary>
        /// Name of the plugin which needed to be reported.
        /// </summary>
        public string PluginName { get; set; }
        /// <summary>
        /// Flag to include buttons
        /// </summary>
        public bool IncludeButtons { get; set; }
        /// <summary>
        /// The exception which occured and needs to be added to the mail.
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// List of the dependecies of the plugin.
        /// </summary>
        public List<string> Dependends { get; set; } 

        /// <summary>
        /// Constructor for the mail class. Fills the properties with the parameters.
        /// </summary>
        /// <param name="target">Name of the target of this mail.</param>
        /// <param name="plugin">Name of the plugin which needed to be reported.</param>
        /// <param name="exc">The exception which occured and needs to be added to the mail.</param>
        /// <param name="includeButtons">Flag to include buttons</param>
        /// <param name="dependendingPlugins">List of the dependecies of the plugin.</param>
        public MailBody(string target, string plugin, Exception exc, bool includeButtons, List<string> dependendingPlugins)
        {
            TargetName = target;
            PluginName = plugin;
            IncludeButtons = includeButtons;
            Exception = exc ?? new Exception("Unknown error");
            Dependends = dependendingPlugins ?? new List<string>();
        }
    }
}
