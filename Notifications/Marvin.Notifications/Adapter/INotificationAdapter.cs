using System.Collections.Generic;

namespace Marvin.Notifications
{
    /// <summary>
    /// Adapter for <see cref="INotificationSender"/> to publish and acknowledge notifications
    /// </summary>
    public interface INotificationAdapter
    {
        /// <summary>
        /// Will return currently published notifications
        /// </summary>
        IReadOnlyList<INotification> GetPublished(INotificationSender sender);

        /// <summary>
        /// Publishes the given notification
        /// </summary>
        void Publish(INotificationSender sender, INotification notification);

        /// <summary>
        /// Acknowledges the given notification
        /// </summary>
        void Acknowledge(INotificationSender sender, INotification notification);

        /// <summary>
        /// Acknowledges all notifications of the given sender
        /// </summary>
        void AcknowledgeAll(INotificationSender sender);
    }
}