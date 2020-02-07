// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;

namespace Moryx.Runtime.Maintenance.Logging
{
    /// <summary>
    /// Model which represens a plugin logger. 
    /// </summary>
    public class LoggerModel
    {
        /// <summary>
        /// Name of this logger.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The level for which this logger is configured. See <see cref="LogLevel"/> for level information.
        /// </summary>
        public LogLevel ActiveLevel { get; set; }

        /// <summary>
        /// List of childs of this logger.
        /// </summary>
        public LoggerModel[] ChildLogger { get; set; }

        /// <summary>
        /// The parent of this logger. 
        /// </summary>
        public LoggerModel Parent { get; set; }
    }
}
