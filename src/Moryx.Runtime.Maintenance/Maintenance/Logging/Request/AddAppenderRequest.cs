// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;

namespace Moryx.Runtime.Maintenance.Logging
{
    /// <summary>
    /// Request to add a logging appender
    /// </summary>
    public class AddAppenderRequest
    {
        /// <summary>
        /// Name of the logger
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Minimal log level for the appender
        /// </summary>
        public LogLevel MinLevel { get; set; }
    }
}
