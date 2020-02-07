// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Logging;

namespace Moryx.Runtime.Maintenance.Logging
{
    /// <summary>
    /// Model which describes a log message.
    /// </summary>
    public class LogMessageModel
    {
        /// <summary>
        /// Instance of a logger.
        /// </summary>
        public LoggerModel Logger { get; set; }

        /// <summary>
        /// Name of the class which created the logger message.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The severity of this message.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// The log message iteself.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Time when the log message occured.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
