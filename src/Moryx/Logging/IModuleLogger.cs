// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.Container;
using Moryx.Modules;

namespace Moryx.Logging
{
    /// <summary>
    /// Different levels to diffentiate the severity of a message
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Information simply used to trace the flow of operations inside a module
        /// </summary>
        Trace,

        /// <summary>
        /// Information used for debugging purposes
        /// </summary>
        Debug,

        /// <summary>
        /// Information about occured events that are nether a warning nor an error
        /// </summary>
        Info,

        /// <summary>
        /// Events that may destabilize the component
        /// </summary>
        Warning,

        /// <summary>
        /// Critical events that may obstruct any further execution
        /// </summary>
        Error,

        /// <summary>
        /// Critical event that does prevent any further component usage
        /// </summary>
        Fatal
    }

    /// <summary>
    /// Logger instance used within one module. All entries will be logged in the modules context.
    /// </summary>
    public interface IModuleLogger : INamedChildContainer<IModuleLogger>
    {
        /// <summary>
        /// Name of this logger instance
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Add a new entry to the log
        /// </summary>
        void Log(LogLevel level, string message, params object[] formatParameters);

        /// <summary>
        /// Log a caught exception
        /// </summary>
        void LogException(LogLevel level, Exception ex, string message, params object[] formatParameters);

        /// <summary>
        /// Define a target for all messages with a level of <see cref="LogLevel.Warning"/> or higher
        /// </summary>
        void SetNotificationTarget(Action<IModuleNotification> notificationTarget);
    }
}
