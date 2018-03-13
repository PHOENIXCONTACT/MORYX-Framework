using System;
using System.Collections.Generic;
using System.Linq;

namespace Marvin.Notifications
{
    public interface INotificationContext
    {
        /// <summary>
        /// Will return currently published notifications by the registered sender
        /// </summary>
        IReadOnlyList<INotification> GetPublished();

        /// <summary>
        /// Raise the published event to inform the NotificationPublisher about a new notification
        /// </summary>
        void Publish(INotification notification);

        /// <summary>
        /// Raise the Acknowledged event to inform the NotificationPublisher that the notification was acknowledged by the registered sender
        /// </summary>
        void Acknowledge(INotification notification);
    }
}
