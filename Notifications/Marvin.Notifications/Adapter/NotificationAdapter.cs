using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Marvin.Container;

namespace Marvin.Notifications
{
    [Plugin(LifeCycle.Singleton, typeof(INotificationAdapter), typeof(INotificationSenderAdapter))]
    public class NotificationAdapter : INotificationAdapter, INotificationSenderAdapter
    {
        private readonly ICollection<NotificationMap> _published = new SynchronizedCollection<NotificationMap>();
        private readonly ICollection<NotificationMap> _pendingAcks = new SynchronizedCollection<NotificationMap>();
        private readonly ICollection<NotificationMap> _pendingPubs = new SynchronizedCollection<NotificationMap>();

        private readonly IDictionary<string, INotificationSender> _senders = new ConcurrentDictionary<string, INotificationSender>();
        #region Adapter <> Publisher

        /// <inheritdoc />
        INotificationContext INotificationAdapter.Register(INotificationSender sender)
        {
            _senders.Add(sender.Identifier, sender);
            return new NotificationContext(sender, this);
        }

        /// <inheritdoc />
        void INotificationAdapter.Unregister(INotificationSender sender)
        {
            _senders.Remove(sender.Identifier);
        }

        /// <inheritdoc />
        internal IReadOnlyList<INotification> GetPublished(INotificationSender sender)
        {
            var notifications = _published.Where(m => m.Sender == sender)
                .Select(map => map.Notification)
                .ToArray();

            return notifications;
        }

        /// <inheritdoc />
        internal void Publish(INotificationSender sender, INotification notification)
        {
            if (!_senders.ContainsKey(sender.Identifier))
                throw new InvalidOperationException("Notification cannot be published. " +
                                                    "The sender was not registered on the adapter");

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
        internal void Acknowledge(INotificationSender sender, INotification notification)
        {
            var managed = (IManagedNotification)notification;
            managed.Acknowledged = DateTime.Now;
            managed.Acknowledger = sender.Identifier;

            var published = _published.SingleOrDefault(n => n.Notification.Identifier == notification.Identifier);
            if (published != null)
            {
                _published.Remove(published);
            }

            if (published == null)
            {
                published = _pendingPubs.SingleOrDefault(n => n.Notification.Identifier == notification.Identifier);

                if (published == null)
                    throw new InvalidOperationException("Notification was not managed by the adapter. " +
                                                        "The sender was not registered on the adapter");

                _pendingPubs.Remove(published);
            }

            _pendingAcks.Add(published);

            Acknowledged?.Invoke(published.Sender, published.Notification);
        }

        #endregion

        #region Publisher <> Adapter

        /// <inheritdoc />
        IReadOnlyList<INotification> INotificationSenderAdapter.GetPublished()
        {
            return _published.Union(_pendingAcks).Select(map => map.Notification).ToArray();
        }

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
        void INotificationSenderAdapter.Sync()
        {
            // Publish pending notifications
            var pendingPublishs = _pendingPubs.ToArray();
            foreach (var pendingPublish in pendingPublishs)
            {
                Published?.Invoke(pendingPublish.Sender, pendingPublish.Notification);
            }

            // Acknowledge pending acknowledges
            foreach (var pendingAck in _pendingAcks)
            {
                Acknowledged?.Invoke(pendingAck.Sender, pendingAck.Notification);
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