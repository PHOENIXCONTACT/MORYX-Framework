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
            get { return Identifier; }
            set { Identifier = value; }
        }

        /// <inheritdoc />
        public DateTime? Acknowledged { get; private set; }

        DateTime? IManagedNotification.Acknowledged
        {
            get { return Acknowledged; }
            set { Acknowledged = value; }
        }

        /// <inheritdoc />
        public string Acknowledger { get; private set; }

        string IManagedNotification.Acknowledger
        {
            get { return Acknowledger; }
            set { Acknowledger = value; }
        }

        /// <inheritdoc />
        public DateTime Created { get; private set; }

        DateTime IManagedNotification.Created
        {
            get { return Created; }
            set { Created = value; }
        }

        /// <inheritdoc />
        string IManagedNotification.Sender { get; set; }

        /// <inheritdoc />
        string IManagedNotification.Source { get; set; }

        /// <inheritdoc />
        public string Type  { get; }

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
            Type = GetType().Name;
        }

        /// <summary>
        /// Creates a new notification with title and message
        /// </summary>
        public Notification(string title, string message, Severity severity)
        {
            Title = title;
            Message = message;
            Severity = severity;
        }
    }
}