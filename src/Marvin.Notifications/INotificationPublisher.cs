using System;

namespace Marvin.Notifications
{
    /// <summary>
    /// Notification publisher facade
    /// </summary>
    public interface INotificationPublisher
    {
        /// <summary>
        /// Returns all current notifications
        /// </summary>
        INotification[] GetAll();

        /// <summary>
        /// Raised if notification was published
        /// </summary>
        event EventHandler<INotification> Published;

        /// <summary>
        /// Raised if notification was acknowledged
        /// </summary>
        event EventHandler<INotification> Acknowledged;
    }
}
