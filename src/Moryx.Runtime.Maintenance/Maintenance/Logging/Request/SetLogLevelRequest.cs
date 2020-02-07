// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;

namespace Moryx.Runtime.Maintenance.Logging
{
    /// <summary>
    /// Request for setting a log level
    /// </summary>
    public class SetLogLevelRequest
    {
        /// <summary>
        /// Log level to set
        /// </summary>
        public LogLevel Level { get; set; }
    }
}
