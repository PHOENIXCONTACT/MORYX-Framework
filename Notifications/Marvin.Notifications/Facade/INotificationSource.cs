using System;
using System.Collections.Generic;

namespace Marvin.Notifications
{
    /// <summary>
    /// Facade interface for providing notifications
    /// </summary>
    public interface INotificationSource
    {
        /// <summary>
        /// Name of the Source which will publish notifications
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Publishes the current state of the facade
        /// </summary>
        bool IsActivated { get; }

        /// <summary>
        /// Returns the currently published notifications
        /// </summary>
        IReadOnlyList<INotification> GetPublished();

        /// <summary>
        /// Restore notification on source
        /// </summary>
        void Sync();

        /// <summary>
        /// Informs the sender of the notification to acknowledge it
        /// </summary>
        void Acknowledge(INotification notification);

        /// <summary>
        /// Called if publisher have processed the notification
        /// </summary>
        void AcknowledgeProcessed(INotification notification);

        /// <summary>
        /// Called if the publisher have processed a publification
        /// </summary>
        void PublishProcessed(INotification notification);

        /// <summary>
        /// Event to publish a notification
        /// </summary>
        event EventHandler<INotification> Published;

        /// <summary>
        /// Event to publish a acknoledged notification to inform the NotificationPublisher about it
        /// </summary>
        event EventHandler<INotification> Acknowledged;

        /// <summary>
        /// Event will be raised if the facade was deactivated
        /// </summary>
        event EventHandler<INotification[]> Deactivated;
    }
}