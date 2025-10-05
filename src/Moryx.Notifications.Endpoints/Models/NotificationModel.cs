// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.Notifications.Endpoints
{
    /// <summary>
    /// Data transfer object for the notification
    /// </summary>
    [DataContract]
    public class NotificationModel
    {
        /// <summary>
        /// The identifier of the notification
        /// </summary>
        [DataMember]
        public Guid Identifier { get; set; }

        /// <summary>
        /// The type of the notification
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// The title of the notification
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The message of the notification
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// The severity of the notification
        /// </summary>
        [DataMember]
        public Severity Severity { get; set; }

        /// <summary>
        /// The creation date of the notification
        /// </summary>
        [DataMember]
        public DateTime Created { get; set; }

        /// <summary>
        /// The time when the notification was acknowledged
        /// </summary>
        [DataMember]
        public DateTime? Acknowledged { get; set; }

        /// <summary>
        /// Indicates whether the notification can be acknowledged
        /// </summary>
        [DataMember]
        public bool IsAcknowledgable { get; set; }

        /// <summary>
        /// Properties of the notification
        /// </summary>
        [DataMember]
        public Entry Properties { get; set; }

        /// <summary>
        /// The sender of the notification
        /// </summary>
        [DataMember]
        public string Sender { get; set; }

        /// <summary>
        /// Source of the notifications
        /// </summary>
        [DataMember]
        public string Source { get; set; }
    }
}
