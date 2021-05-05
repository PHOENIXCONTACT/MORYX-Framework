// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Runtime.Maintenance.Plugins.Logging;

namespace Moryx.Runtime.Kestrel.Maintenance
{
    /// <summary>
    /// Configuration for the logging of the maintenance.
    /// </summary>
    [DataContract]
    public class LoggingMaintenanceConfig : IPluginConfig
    {
        /// <inheritdoc />
        public string PluginName => LoggingAppenderPlugin.PluginName;

        /// <summary>
        /// If a logging appender exceeds this timeouts its config stream is closed
        /// </summary>
        [DataMember]
        [Description("If a logging appender exceeds this timeouts its config stream is closed")]
        [DefaultValue(30000)]
        public int AppenderTimeOut { get; set; }
    }
}
