using System;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Notifications;

namespace Marvin.Runtime.Modules
{
    internal class ModuleNotification : IModuleNotification
    {
        private readonly Action<ModuleNotification> _confirmationDelegate;

        private static void EmptyDelegate(ModuleNotification notification)
        {
        }

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

        public ModuleNotification(Severity severity, string message)
            : this(severity, message, null, EmptyDelegate)
        {
        }

        public ModuleNotification(Severity severity, string message, Action<ModuleNotification> confirmationDelegate) 
            : this(severity, message, null, confirmationDelegate)
        {
        }

        public ModuleNotification(Severity severity, string message, Exception exception) 
            : this(severity, message, exception, EmptyDelegate)
        {
        }

        public ModuleNotification(Severity severity, string message, Exception exception, Action<ModuleNotification> confirmationDelegate)
        {
            Severity = severity;
            Message = message;
            Timestamp = DateTime.Now;
            Exception = exception;

            _confirmationDelegate = confirmationDelegate;
        }

        public ModuleNotification(LogLevel logLevel, string message, Exception exception)
            : this(LogLevelToSeverity(logLevel), message, exception, EmptyDelegate)
        {
        }

        public ModuleNotification(LogLevel logLevel, string message, Exception exception, Action<ModuleNotification> confirmationDelegate)
            : this(LogLevelToSeverity(logLevel), message, exception, confirmationDelegate)
        {
        }

        public bool Confirm()
        {
            _confirmationDelegate?.Invoke(this);
            return true;
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
