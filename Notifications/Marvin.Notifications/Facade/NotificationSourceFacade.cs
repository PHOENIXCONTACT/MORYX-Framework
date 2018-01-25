using System;
using System.Collections.Generic;
using Marvin.Runtime.Modules;

namespace Marvin.Notifications
{
    /// <summary>
    /// Default implementation of the <see cref="INotificationSource"/> facade
    /// </summary>
    public class NotificationSourceFacade : FacadeBase, INotificationSource
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Creates a new named notification source facade
        /// </summary>
        public NotificationSourceFacade(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The Adapter to interact with the notification senders
        /// </summary>
        public INotificationSenderAdapter NotificationAdapter { get; set; }

        /// <inheritdoc />
        public override void Activate()
        {
            base.Activate();

            NotificationAdapter.Published += OnNotificationPublished;
            NotificationAdapter.Acknowledged += OnNotificationAcknowledged;
        }

        /// <inheritdoc />
        public override void Deactivate()
        {
            NotificationAdapter.Published -= OnNotificationPublished;
            NotificationAdapter.Acknowledged -= OnNotificationAcknowledged;

            base.Deactivate();
        }

        /// <inheritdoc />
        public void Sync(IReadOnlyList<INotification> notifications)
        {
            ValidateHealthState();
            NotificationAdapter.Sync(notifications);
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
    }
}