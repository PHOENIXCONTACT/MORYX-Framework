using System;
using System.Collections.Generic;

namespace Marvin.Notifications
{
    internal class NotificationContext : INotificationContext
    {
        private INotificationSender _sender;
        private NotificationAdapter _adapter;

        public NotificationContext(INotificationSender sender, NotificationAdapter adapter)
        {
            _sender = sender;
            _adapter = adapter;
        }

        /// <inheritdoc />
        public IReadOnlyList<INotification> GetPublished()
        {
            return _adapter.GetPublished(_sender);
        }

        /// <inheritdoc />
        public void Publish(INotification notification)
        {
            _adapter.Publish(_sender, notification);
        }

        /// <inheritdoc />
        public void Acknowledge(INotification notification)
        {
            _adapter.Acknowledge(_sender, notification);
        }
    }
}
