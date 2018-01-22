using System;
using System.Collections.Generic;

namespace Marvin.Notifications
{
    public interface INotificationSenderAdapter
    {
        /// <summary>
        /// Informs the sender of the notification to acknowledge it
        /// </summary>
        void Acknowledge(INotification notification);

        /// <summary>
        /// Restore notification on sender adapter
        /// </summary>
        void Sync(IReadOnlyList<INotification> notifications);

        /// <summary>
        /// Event to publish a notification
        /// </summary>
        event EventHandler<INotification> Published;

        /// <summary>
        /// Event to publish an acknowledged notification
        /// </summary>
        event EventHandler<INotification> Acknowledged;

        /// <summary>
        /// Informs a sender of an acknowledgement that it was processed
        /// </summary>
        void AcknowledgeProcessed(INotification notification);

        /// <summary>
        /// Informs a sender of an notification that it was processed
        /// </summary>
        void PublishProcessed(INotification notification);
    }
}