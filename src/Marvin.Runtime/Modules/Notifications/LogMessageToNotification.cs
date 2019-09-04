using System;
using Marvin.Logging;
using Marvin.Notifications;

namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Static helper to convert log messages to notifications
    /// </summary>
    internal static class LogMessageToNotification
    {
        public static ModuleNotification Convert(ILogMessage message, Action<ModuleNotification> confirmation)
        {
            var severity = LogLevelToSeverity(message.Level);
            return new ModuleNotification(severity, message.Message, message.Exception, confirmation);
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