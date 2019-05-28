using System;
using Marvin.Modules;
using Marvin.Notifications;

namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    /// <summary>
    /// Model for notifications.
    /// </summary>
    public class NotificationModel
    {
        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public NotificationModel()
        { 
            
        }

        /// <summary>
        /// Constructor for a notification.
        /// </summary>
        /// <param name="notification">A notification raisded by a module.</param>
        public NotificationModel(IModuleNotification notification)
        {
            Timestamp = notification.Timestamp;
            if(notification.Exception != null)
                Exception = new SerializableException(notification.Exception);
            Severity = notification.Severity;
        }

        /// <summary>
        /// The timestamp when the notofication occured..
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// An exception.
        /// </summary>
        public SerializableException Exception { get; set; }

        /// <summary>
        /// Kind of notification
        /// </summary>
        public Severity Severity { get; set; }
    }
}
