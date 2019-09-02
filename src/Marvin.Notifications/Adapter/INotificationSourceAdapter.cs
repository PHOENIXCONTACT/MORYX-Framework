using System;
using System.Collections.Generic;

namespace Marvin.Notifications
{
    /// <summary>
    /// Adapter for the <see cref="INotificationSource"/>
    /// </summary>
    public interface INotificationSourceAdapter
    {
        /// <summary>
        /// Returns the currently published notifications
        /// </summary>
        IReadOnlyList<INotification> GetPublished();

        /// <summary>
        /// Informs the sender of the notification to acknowledge it
        /// </summary>
        void Acknowledge(INotification notification);

        /// <summary>
        /// Restore notification on sender adapter
        /// </summary>
        void Sync();

        /// <summary>
        /// Informs a sender of an acknowledgement that it was processed
        /// </summary>
        void AcknowledgeProcessed(INotification notification);

        /// <summary>
        /// Informs a sender of an notification that it was processed
        /// </summary>
        void PublishProcessed(INotification notification);

        /// <summary>
        /// Will be called when a foreign notification was published
        /// </summary>
        void PublishedForeign(INotification notification);

        /// <summary>
        /// Will be called when a foreign notification was published
        /// </summary>
        void AcknowledgedForeign(INotification notification);

        /// <summary>
        /// Event to publish a notification
        /// </summary>
        event EventHandler<INotification> Published;

        /// <summary>
        /// Event to publish an acknowledged notification
        /// </summary>
        event EventHandler<INotification> Acknowledged;
    }
}