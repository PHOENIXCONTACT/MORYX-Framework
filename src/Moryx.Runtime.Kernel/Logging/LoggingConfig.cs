// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Logging;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// config for logging.
    /// </summary>
    [DataContract]
    public class LoggingConfig : ConfigBase
    {
        /// <summary>
        /// Config entry to set the default level of the logging.
        /// </summary>
        [DataMember]
        [DefaultValue(LogLevel.Info)]
        public LogLevel DefaultLevel { get; set; }

        /// <summary>
        /// List of underlying ModuleLoggerConfigs.
        /// </summary>
        [DataMember]
        public List<ModuleLoggerConfig> LoggerConfigs { get; set; }
    }
}
