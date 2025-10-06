// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.Notifications;

namespace Moryx.Runtime.Endpoints.Modules.Endpoint.Models
{
    /// <summary>
    /// Model for notifications.
    /// </summary>
    public class ModuleNotificationModel
    {
        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public ModuleNotificationModel()
        {

        }

        /// <summary>
        /// Constructor for a notification.
        /// </summary>
        /// <param name="notification">A notification raisded by a module.</param>
        public ModuleNotificationModel(IModuleNotification notification)
        {
            Timestamp = notification.Timestamp;
            if (notification.Exception != null)
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
