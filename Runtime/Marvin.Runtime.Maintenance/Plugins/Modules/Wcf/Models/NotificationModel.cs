using System;
using Marvin.Modules;

namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    /// <summary>
    /// Model for notifications.
    /// </summary>
    public class NotificationModel
    {
        /// <summary>
        /// Constructor for a notification.
        /// </summary>
        /// <param name="notification">A notification raisded by a module.</param>
        public NotificationModel(IModuleNotification notification)
        {
            Timestamp = notification.Timestamp;
            //todo: when is a notification important?
            //Important = notification.ImportantNotification;
            Exception = new SerializableException(notification.Exception);
            NotificationType = notification.Type;
        }

        /// <summary>
        /// The timestamp when the notofication occured..
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Flag if this notification is important.
        /// </summary>
        public bool Important { get; set; }

        /// <summary>
        /// An exception.
        /// </summary>
        public SerializableException Exception { get; set; }

        /// <summary>
        /// Kind of notification
        /// </summary>
        public NotificationType NotificationType { get; set; }
    }
}
