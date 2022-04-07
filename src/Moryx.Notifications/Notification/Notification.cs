// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Notifications
{
    /// <summary>
    /// Base class of all notifications.
    /// </summary>
    public class Notification : IManagedNotification
    {
        /// <inheritdoc />
        public virtual Guid Identifier { get; private set; }

        /// <inheritdoc cref="INotification"/> />
        public Severity Severity { get; set; }

        /// <inheritdoc cref="INotification"/> />
        public string Title { get; set; }

        /// <inheritdoc cref="INotification"/> />
        public string Message { get; set; }

        /// <inheritdoc cref="INotification"/> />
        public string Sender { get; set; }

        /// <inheritdoc cref="INotification"/> />
        public string Source { get; set; }

        /// <inheritdoc cref="INotification"/> />
        public bool IsAcknowledgable { get; set; }

        // TODO: AL6 remove explicit backing attribute for property
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
                    throw new InvalidOperationException("Tried to update time of acknowledgement.");
            }
        }

        // TODO: AL6 remove explicit backing attribute for property
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
                    throw new InvalidOperationException("Tried to update acknowledger.");
            }
        }

        // TODO: AL6 Remove backing attribute for property and make property nullable
        private DateTime? _created;
        /// <inheritdoc />
        public virtual DateTime Created
        {
            get => _created ?? default(DateTime);
            set
            {
                if (_created is null)
                    _created = value;
                else
                    throw new InvalidOperationException("Tried to update creation time.");
            }
        }

        /// <summary>
        /// Creates a new notification
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

        #region IManagedNotification
        /// <inheritdoc />
        Guid IManagedNotification.Identifier
        {
            get => Identifier;
            set => Identifier = value;
        }

        DateTime? IManagedNotification.Acknowledged
        {
            get => _acknowledged;
            set => _acknowledged = value;
        }

        string IManagedNotification.Acknowledger
        {
            get => Acknowledger;
            set => Acknowledger = value;
        }

        DateTime IManagedNotification.Created
        {
            get => Created;
            set => Created = value;
        }

        /// <inheritdoc />
        string IManagedNotification.Sender { get; set; }

        /// <inheritdoc />
        string IManagedNotification.Source { get; set; }
        #endregion
    }
}
