using System;

namespace Marvin.Notifications
{
    /// <summary>
    /// Base class of all notifications.
    /// </summary>
    public class Notification : IManagedNotification
    {
        /// <inheritdoc />
        public string Identifier { get; private set; }

        /// <inheritdoc />
        string IManagedNotification.Identifier
        {
            get => Identifier;
            set => Identifier = value;
        }

        /// <inheritdoc />
        public DateTime? Acknowledged { get; private set; }

        DateTime? IManagedNotification.Acknowledged
        {
            get => Acknowledged;
            set => Acknowledged = value;
        }

        /// <inheritdoc cref="IManagedNotification.IsAcknowledgable" />
        public bool IsAcknowledgable { get; set; }

        /// <inheritdoc />
        public string Acknowledger { get; private set; }

        string IManagedNotification.Acknowledger
        {
            get => Acknowledger;
            set => Acknowledger = value;
        }

        /// <inheritdoc />
        public DateTime Created { get; private set; }

        DateTime IManagedNotification.Created
        {
            get => Created;
            set => Created = value;
        }

        /// <inheritdoc />
        string IManagedNotification.Sender { get; set; }

        /// <inheritdoc />
        string IManagedNotification.Source { get; set; }

        /// <inheritdoc />
        public Severity Severity { get; set; }

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
        public string Message { get; set; }

        /// <summary>
        /// Creates a new notification
        /// </summary>
        public Notification()
        {
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