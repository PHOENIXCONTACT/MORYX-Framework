// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Logging
{
    /// <summary>
    /// Log message interface with readonly access
    /// </summary>
    public interface ILogMessage
    {
        /// <summary>
        /// Name of the logger that created the message
        /// </summary>
        IModuleLogger Logger { get; }
        /// <summary>
        /// Logging class
        /// </summary>
        string ClassName { get; }
        /// <summary>
        /// Level of the message
        /// </summary>
        LogLevel Level { get; }
        /// <summary>
        /// The logged message
        /// </summary>
        string Message { get; }
        /// <summary>
        /// Optionally the exception that triggered the log message
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// The moment this message was passed to the logger
        /// </summary>
        DateTime Timestamp { get; }
    }
}
