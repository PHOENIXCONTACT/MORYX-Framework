using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Logging;

namespace Marvin.Runtime.Kernel.Logging
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
        /// List of underliing ModuleLoggerConfigs.
        /// </summary>
        [DataMember]
        public List<ModuleLoggerConfig> LoggerConfigs { get; set; }
    }
}