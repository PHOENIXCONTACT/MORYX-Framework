using System;
using System.Collections.Generic;
using System.Diagnostics;
using Marvin.Runtime.Modules;

namespace Marvin.Notifications
{
    /// <summary>
    /// Facade implementation which uses the <see cref="INotificationSourceAdapter"/> to
    /// provide the facade api <see cref="INotificationSource"/>
    /// </summary>
    [DebuggerDisplay("NotificationSource: {" + nameof(Name) + "}")]
    public class NotificationSourceFacade : FacadeBase, INotificationSource
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Creates a new named notification source facade
        /// </summary>
        /// <summary>
        /// Constructor to create a new instance of <see cref="NotificationSourceFacade"/>
        /// </summary>
        /// <param name="name"></param>
        public NotificationSourceFacade(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The Adapter to interact with the notification senders
        /// </summary>
        public INotificationSourceAdapter NotificationAdapter { get; set; }

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
        public void PublishedForeign(INotification notification)
        {
            ValidateHealthState();
            NotificationAdapter.PublishedForeign(notification);
        }

        /// <inheritdoc />
        public void AcknowledgedForeign(INotification notification)
        {
            ValidateHealthState();
            NotificationAdapter.AcknowledgedForeign(notification);
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