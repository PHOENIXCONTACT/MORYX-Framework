// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Notifications;

namespace Moryx.Runtime.Kernel.Logging
{
    internal class ModuleNotification : IModuleNotification
    {
        /// <summary>
        /// Type of this notification
        /// </summary>
        public Severity Severity { get; }

        /// <summary>
        /// Notification message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Time stamp of occurence
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Optional exception as cause of this message
        /// </summary>
        public Exception Exception { get; }

        public ModuleNotification(LogLevel logLevel, string message, Exception exception)
        {
            Severity = LogLevelToSeverity(logLevel);
            Message = message;
            Timestamp = DateTime.Now;
            Exception = exception;
        }

        private static Severity LogLevelToSeverity(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Info:
                    return Severity.Info;
                case LogLevel.Warning:
                    return Severity.Warning;
                case LogLevel.Error:
                    return Severity.Error;
                case LogLevel.Fatal:
                    return Severity.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}
