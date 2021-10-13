// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

#if USE_WCF
using Moryx.Tools.Wcf;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Logging
{
    /// <summary>
    /// Configuration for the logging of the maintenance.
    /// </summary>
    [DataContract]
    internal class LoggingMaintenanceConfig : MaintenancePluginConfig
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string PluginName => LoggingAppenderPlugin.PluginName;

        /// <summary>
        /// Constructor for the logging maintenance config. Creates an endpoint "LogMaintenance" with binding type "BasicHttp"
        /// </summary>
        public LoggingMaintenanceConfig()
        {
#if USE_WCF
            ProvidedEndpoint = new HostConfig
            {
                Endpoint = "loggers",
                BindingType = ServiceBindingType.WebHttp,
                MetadataEnabled = true
            };
#endif
        }

        /// <summary>
        /// If a logging appender exceeds this timeouts its config stream is closed
        /// </summary>
        [DataMember]
        [Description("If a logging appender exceeds this timeouts its config stream is closed")]
        [DefaultValue(30000)]
        public int AppenderTimeOut { get; set; }
    }
}
