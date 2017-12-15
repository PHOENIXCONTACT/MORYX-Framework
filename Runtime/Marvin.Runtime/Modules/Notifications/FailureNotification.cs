using System;
using Marvin.Modules;

namespace Marvin.Runtime.Modules
{
    internal class FailureNotification : IModuleNotification
    {
        /// <summary>
        /// Create notification using only an exception. Exception message is used as message
        /// </summary>
        /// <param name="ex">Exception that caused the failure</param>
        public FailureNotification(Exception ex) : this(ex, ex.Message)
        {
        }

        /// <summary>
        /// Create notification using an exception and additional message
        /// </summary>
        /// <param name="ex">Exception that caused the failure</param>
        /// <param name="message">Additional message</param>
        public FailureNotification(Exception ex, string message)
        {
            Type = NotificationType.Failure;
            Timestamp = DateTime.Now;
            Message = message;
            Exception = ex;
        }

        /// <summary>
        /// Confirm acknowledgement of this notification
        /// </summary>
        /// <returns>
        /// True of message could be confirmed
        /// </returns>
        public bool Confirm()
        {
            // Failure can not be confirmed
            return false;
        }

        /// <summary>
        /// Type of this notification
        /// </summary>
        public NotificationType Type { get; }

        /// <summary>
        /// Time stamp of occurence
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Notification message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Optional exception as cause of this message
        /// </summary>
        public Exception Exception { get; }
    }
}
