// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Modules;
using Moryx.Notifications;

namespace Moryx.Runtime.Maintenance.Modules
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
            Message = notification.Message;
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
        /// Notification message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Kind of notification
        /// </summary>
        public Severity Severity { get; set; }
    }
}
