// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Logging
{
    /// <summary>
    /// Framework component managing the life cycle of all loggers and provide diagnostic access to them
    /// </summary>
    public interface ILoggerManagement : IEnumerable<IModuleLogger>
    {
        /// <summary>
        /// Activate logging for a Module
        /// </summary>
        /// <param name="module"></param>
        void ActivateLogging(ILoggingHost module);

        /// <summary>
        /// Deactivate logging for a Module
        /// </summary>
        /// <param name="module"></param>
        void DeactivateLogging(ILoggingHost module);

        /// <summary>
        /// Set log level of logger
        /// </summary>
        void SetLevel(IModuleLogger logger, LogLevel level);

        /// <summary>
        /// Set log level of logger by its name
        /// </summary>
        void SetLevel(string name, LogLevel level);
    }
}
