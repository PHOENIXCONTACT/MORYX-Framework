// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Logging;
using Moryx.Notifications;

namespace Moryx.Runtime.Notifications
{
    /// <summary>
    /// Static helper to convert log messages to notifications
    /// </summary>
    internal static class LogMessageToNotification
    {
        public static ModuleNotification Convert(ILogMessage message)
        {
            var severity = LogLevelToSeverity(message.Level);
            return new ModuleNotification
            {
                Severity = severity,
                Message = message.Message,
                Exception = message.Exception
            };
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