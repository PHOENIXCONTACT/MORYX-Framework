using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Marvin.Notifications;

namespace Marvin.Modules
{
    /// <summary>
    /// Notification raised by a module. May disappear automatically or has to be acknowledged explicitly
    /// </summary>
    public interface IModuleNotification
    {
        /// <summary>
        /// Type of this notification
        /// </summary>
        Severity Severity { get; }

        /// <summary>
        /// Confirm acknowledgement of this notification
        /// </summary>
        /// <returns>True of message could be confirmed</returns>
        bool Confirm();

        /// <summary>
        /// Time stamp of occurence
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Notification message
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Optional exception as cause of this message
        /// </summary>
        Exception Exception { get; }
    }

    /// <summary>
    /// Collection of notifications
    /// </summary>
    public interface INotificationCollection : ICollection<IModuleNotification>, INotifyCollectionChanged
    {
    }
}
