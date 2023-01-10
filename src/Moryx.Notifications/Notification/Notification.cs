// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Notifications
{
    /// <summary>
    /// Base class of all notifications.
    /// </summary>
    public class Notification 
    {
        /// <summary>
        /// Unique identifier of this notification
        /// </summary>
        public virtual Guid Identifier { get; set; }

        /// <summary>
        /// The severity of this notification
        /// </summary>
        public Severity Severity { get; set; }

        /// <summary>
        /// Optional title of this notification. Can be set by processor as well
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Message of this notification.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Sender of this notification. <see cref="INotificationSender"/>
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Source of this notification. <see cref="INotificationSource"/>
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Indicates is the notification can be acknowledged
        /// </summary>
        public bool IsAcknowledgable { get; set; }

        /// <summary>
        /// Contains the date and time when this notification has been acknowledged. 
        /// Is Null, if the notification has not yet been acknowledged.
        /// </summary>
        private DateTime? _acknowledged;
        /// <inheritdoc />
        public virtual DateTime? Acknowledged
        {
            get => _acknowledged;
            set
            {
                if (_acknowledged is null)
                    _acknowledged = value;
                else
                    throw new InvalidOperationException($"Tried to update {nameof(Acknowledged)}.");
            }
        }

        /// <summary>
        /// Who or what acknowledged the notification, if it was acknowledged.
        /// <see cref="Acknowledged"/> shows if the notification has been acknowledged.
        /// </summary>
        private string _acknowledger;
        /// <inheritdoc />
        public virtual string Acknowledger
        {
            get => _acknowledger;
            set
            {
                if (_acknowledger is null)
                    _acknowledger = value;
                else
                    throw new InvalidOperationException($"Tried to update {nameof(Acknowledger)}.");
            }
        }

        /// <summary>
        /// Date of creation
        /// </summary>
        private DateTime? _created;
        /// <inheritdoc />
        public virtual DateTime? Created
        {
            get => _created;
            set
            {
                if (_created is null)
                    _created = value;
                else
                    throw new InvalidOperationException($"Tried to update {nameof(Created)}.");
            }
        }

        /// <summary>
        /// Creates a new notification with automatically set <see cref="Identifier"/>.
        /// </summary>
        public Notification()
        {
            Identifier = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new notification with title and message
        /// </summary>
        public Notification(string title, string message, Severity severity) : this()
        {
            Title = title;
            Message = message;
            Severity = severity;
        }

        /// <summary>
        /// Creates a new notification with title and message
        /// </summary>
        public Notification(string title, string message, Severity severity, bool isAcknowledgable) : this(title, message, severity)
        {
            IsAcknowledgable = isAcknowledgable;
        }
    }
}
