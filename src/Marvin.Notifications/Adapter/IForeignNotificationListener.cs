using System;

namespace Marvin.Notifications
{
    /// <summary>
    /// Interface to listen on notifications for foreign notifications which are not published by the adapter
    /// </summary>
    public interface IForeignNotificationListener
    {
        /// <summary>
        /// Event if a foreign notification was published
        /// </summary>
        event EventHandler<INotification> ForeignPublished;

        /// <summary>
        /// Event if a foreign notification was acknowledged
        /// </summary>
        event EventHandler<INotification> ForeignAcknowledged;
    }
}