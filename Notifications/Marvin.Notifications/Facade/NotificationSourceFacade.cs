using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Marvin.Runtime.Base;

namespace Marvin.Notifications
{
    /// <summary>
    /// Facade implementation which uses the <see cref="INotificationSourceAdapter"/> to
    /// provide the facade api <see cref="INotificationSource"/>
    /// </summary>
    [DebuggerDisplay("NotificationSource: {" + nameof(Name) + "}")]
    public class NotificationSourceFacade : INotificationSource, IFacadeControl
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool IsActivated { get; private set; }

        /// <summary>
        /// Constructor to create a new instance of <see cref="NotificationSourceFacade"/>
        /// </summary>
        /// <param name="name"></param>
        public NotificationSourceFacade(string name)
        {
            Name = name;
        }

        /// <inheritdoc />
        public Action ValidateHealthState { get; set; }

        /// <summary>
        /// The Adapter to interact with the notification senders
        /// </summary>
        public INotificationSourceAdapter NotificationAdapter { get; set; }

        /// <inheritdoc />
        public void Activate()
        {
            IsActivated = true;
            NotificationAdapter.Published += OnNotificationPublished;
            NotificationAdapter.Acknowledged += OnNotificationAcknowledged;
        }

        /// <inheritdoc />
        public void Deactivate()
        {
            NotificationAdapter.Published -= OnNotificationPublished;
            NotificationAdapter.Acknowledged -= OnNotificationAcknowledged;

            // TODO: Raise this Event when the facade was deactivated not while deactivate. Wait for Platform 3.0.
            IsActivated = false;
            Deactivated?.Invoke(this, NotificationAdapter.GetPublished().ToArray());
        }

        /// <inheritdoc />
        public IReadOnlyList<INotification> GetPublished()
        {
            ValidateHealthState();
            return NotificationAdapter.GetPublished();
        }

        /// <inheritdoc />
        public void Sync()
        {
            ValidateHealthState();
            NotificationAdapter.Sync();
        }

        /// <inheritdoc />
        public void Acknowledge(INotification notification)
        {
            ValidateHealthState();
            NotificationAdapter.Acknowledge(notification);
        }

        /// <inheritdoc />
        public void PublishProcessed(INotification notification)
        {
            ValidateHealthState();
            NotificationAdapter.PublishProcessed(notification);
        }

        /// <inheritdoc />
        public void AcknowledgeProcessed(INotification notification)
        {
            ValidateHealthState();
            NotificationAdapter.AcknowledgeProcessed(notification);
        }

        private void OnNotificationPublished(object sender, INotification notification)
        {
            // Add this facade to the notification to find it on restore
            var managed = (IManagedNotification) notification;
            managed.Source = Name;

            Published?.Invoke(this, notification);
        }

        private void OnNotificationAcknowledged(object sender, INotification notification)
        {
            Acknowledged?.Invoke(this, notification);
        }

        /// <inheritdoc />
        public event EventHandler<INotification> Published;

        /// <inheritdoc />
        public event EventHandler<INotification> Acknowledged;

        /// <inheritdoc />
        public event EventHandler<INotification[]> Deactivated;
    }
}