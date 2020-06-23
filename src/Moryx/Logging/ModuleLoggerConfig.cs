// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Moryx.Logging
{
    /// <summary>
    /// Configuration for the module logger module.
    /// </summary>
    [DataContract]
    public class ModuleLoggerConfig
    {
        /// <summary>
        /// Constructor of the ModuleLoggerConfig.
        /// </summary>
        public ModuleLoggerConfig()
        {
            ActiveLevel = LogLevel.Info;
            ChildConfigs = new List<ModuleLoggerConfig>();
        }

        /// <summary>
        /// Name of this logger.
        /// </summary>
        [DataMember]
        public string LoggerName { get; set; }

        /// <summary>
        /// The active level of the logger. <see cref="LogLevel"/> for details about the log levels.
        /// </summary>
        [DataMember]
        public LogLevel ActiveLevel { get; set; }

        /// <summary>
        /// List of child configurations of ModuleLoggerConfigs.
        /// </summary>
        [DataMember]
        public List<ModuleLoggerConfig> ChildConfigs { get; set; } 
    }
}
