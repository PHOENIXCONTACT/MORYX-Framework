using System.Collections.Generic;

namespace Marvin.Notifications
{
    public interface INotificationAdapter
    {
        /// <summary>
        /// Registers the sender at the adapter
        /// </summary>
        /// <param name="sender"></param>
        void Register(INotificationSender sender);

        /// <summary>
        /// Removes the registration of the sender
        /// </summary>
        /// <param name="sender"></param>
        void Unregister(INotificationSender sender);

        /// <summary>
        /// Will return currently published notifications by the given sender
        /// </summary>
        IReadOnlyList<INotification> GetPublished(INotificationSender sender);

        /// <summary>
        /// Raise the published event to inform the NotificationPublisher about a new notification
        /// </summary>
        void Publish(INotificationSender sender, INotification notification);

        /// <summary>
        /// Raise the Acknowledged event to inform the NotificationPublisher that the notification was acknowledged by the sender
        /// </summary>
        void Acknowledge(INotification notification);
    }
}