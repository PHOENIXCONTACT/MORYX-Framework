using System;
using System.Collections.Generic;
using Marvin.Runtime.Base;

namespace Marvin.Notifications
{
    public class NotificationSourceFacade : INotificationSource, IFacadeControl
    {
        /// <summary>
        /// Name of this facade
        /// </summary>
        public string Name { get; }

        public NotificationSourceFacade(string name)
        {
            Name = name;
        }

        /// <inheritdoc />
        public Action ValidateHealthState { get; set; }

        /// <summary>
        /// The Adapter to interact with the notification senders
        /// </summary>
        public INotificationSenderAdapter NotificationAdapter { get; set; }

        /// <inheritdoc />
        public void Activate()
        {
            NotificationAdapter.Published += OnNotificationPublished;
            NotificationAdapter.Acknowledged += OnNotificationAcknowledged;
        }
        
        /// <inheritdoc />
        public void Deactivate()
        {
            NotificationAdapter.Published -= OnNotificationPublished;
            NotificationAdapter.Acknowledged -= OnNotificationAcknowledged;
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