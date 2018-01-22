using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Container;

namespace Marvin.Notifications
{
    [Plugin(LifeCycle.Singleton, typeof(INotificationAdapter), typeof(INotificationSenderAdapter))]
    public class NotificationAdapter : INotificationAdapter, INotificationSenderAdapter
    {
        private readonly ICollection<NotificationMap> _published = new List<NotificationMap>();
        private readonly ICollection<NotificationMap> _pendingAcks = new List<NotificationMap>();
        private readonly ICollection<NotificationMap> _pendingPubs = new List<NotificationMap>();

        private readonly IDictionary<string, INotificationSender> _senders = new Dictionary<string, INotificationSender>();

        #region Adapter <> Publisher

        /// <inheritdoc />
        void INotificationAdapter.Register(INotificationSender sender)
        {
            lock (_senders)
                _senders.Add(sender.Identifier, sender);
        }

        /// <inheritdoc />
        void INotificationAdapter.Unregister(INotificationSender sender)
        {
            lock (_senders)
                _senders.Remove(sender.Identifier);
        }

        /// <inheritdoc />
        IReadOnlyList<INotification> INotificationAdapter.GetPublished(INotificationSender sender)
        {
            var notifications = _published.Where(m => m.Sender == sender)
                .Select(map => map.Notification)
                .ToArray();

            return notifications;
        }

        /// <inheritdoc />
        void INotificationAdapter.Publish(INotificationSender sender, INotification notification)
        {
            var managed = (IManagedNotification)notification;
            managed.Identifier = Guid.NewGuid().ToString();
            managed.Created = DateTime.Now;
            managed.Sender = sender.Identifier;

            // Lets check if the notification was already published
            var isPending = _pendingPubs.Union(_pendingAcks).Union(_published)
                .Any(n => n.Notification.Identifier.Equals(notification.Identifier));

            if (isPending)
            {
                throw new InvalidOperationException("Notification cannot be published twice!");
            }
            
            _pendingPubs.Add(new NotificationMap(sender, notification));

            Published?.Invoke(sender, notification);
        }

        /// <inheritdoc />
        void INotificationAdapter.Acknowledge(INotification notification)
        {
            var managed = (IManagedNotification)notification;
            managed.Acknowledged = DateTime.Now;

            var published = _published.SingleOrDefault(n => n.Notification.Identifier == notification.Identifier);
            if (published != null)
            {
                _published.Remove(published);
            }

            if (published == null)
            {
                published = _pendingPubs.Single(n => n.Notification.Identifier == notification.Identifier);
                _pendingPubs.Remove(published);
            }

            _pendingAcks.Add(published);

            Acknowledged?.Invoke(published.Sender, published.Notification);
        }

        #endregion

        #region Publisher <> Adapter

        /// <inheritdoc />
        void INotificationSenderAdapter.Acknowledge(INotification notification)
        {
            var map = _published.Single(m => m.Notification.Identifier == notification.Identifier);
            map.Sender.Acknowledge(map.Notification);
        }

        /// <inheritdoc />
        void INotificationSenderAdapter.AcknowledgeProcessed(INotification notification)
        {
            var map = _pendingAcks.Single(n => n.Notification.Identifier.Equals(notification.Identifier));
            _pendingAcks.Remove(map);
        }

        /// <inheritdoc />
        void INotificationSenderAdapter.PublishProcessed(INotification notification)
        {
            var map = _pendingPubs.Single(n => n.Notification.Identifier.Equals(notification.Identifier));

            _pendingPubs.Remove(map);
            _published.Add(map);
        }

        /// <inheritdoc />
        void INotificationSenderAdapter.Sync(IReadOnlyList<INotification> restored)
        {
            // Publish pending notifications
            var pendingPublishs = _pendingPubs.ToArray();
            foreach (var pendingPublish in pendingPublishs)
            {
                Published?.Invoke(pendingPublish.Sender, pendingPublish.Notification);
            }

            // Acknowledge pending acknowledges
            foreach (var notification in restored)
            {
                var pendingAck = _pendingAcks.SingleOrDefault(map => map.Notification.Identifier == notification.Identifier);
                if (pendingAck != null)
                {
                    Acknowledged?.Invoke(pendingAck.Sender, pendingAck.Notification);
                    continue;
                }

                var managed = (IManagedNotification)notification;
                var existing = _published.SingleOrDefault(m => m.Notification.Identifier == notification.Identifier);
                if (existing != null)
                    continue; // Notification is already available in this adapter

                INotificationSender sender = null;
                lock (_senders)
                {
                    if (_senders.ContainsKey(managed.Sender))
                        sender = _senders[managed.Sender];
                }
                    
                if (sender == null)
                {
                    // Sender cannot be found anymore. Notification will be acknowledged
                    managed.Acknowledged = DateTime.Now;

                    _pendingAcks.Add(new NotificationMap(null, notification));
                    Acknowledged?.Invoke(this, notification);
                    return;
                }

                _published.Add(new NotificationMap(sender, notification)); 
            }
        }

        /// <inheritdoc />
        public event EventHandler<INotification> Published;

        /// <inheritdoc />
        public event EventHandler<INotification> Acknowledged;

        #endregion

        private class NotificationMap
        {
            public NotificationMap(INotificationSender sender, INotification notification)
            {
                Sender = sender;
                Notification = notification;
            }

            public INotification Notification { get; }

            public INotificationSender Sender { get; }
        }
    }
}