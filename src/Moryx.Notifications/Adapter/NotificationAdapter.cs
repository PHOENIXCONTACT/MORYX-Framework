// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;

namespace Moryx.Notifications
{
    /// <summary>
    /// Notification adapter for the server module.
    /// The events and calls will be redirected to the <see cref="INotificationSource"/>
    /// </summary>
    public class NotificationAdapter : INotificationAdapter, INotificationSourceAdapter
    {
        private readonly List<NotificationMap> _published = new(16);
        private readonly List<NotificationMap> _pendingAcks = new(16);
        private readonly List<NotificationMap> _pendingPubs = new(16);

        private readonly object _listLock = new();

        /// <summary>
        /// Logger used by the <see cref="NotificationAdapter"/>
        /// </summary>
        public ILogger Logger { get; set; }

        #region Adapter <> Facade

        /// <inheritdoc />
        public IReadOnlyList<Notification> GetPublished(INotificationSender sender)
        {
            return GetPublished(map => map.Sender == sender);
        }

        /// <inheritdoc />
        public IReadOnlyList<Notification> GetPublished(INotificationSender sender, object tag)
        {
            return GetPublished(map => map.Sender == sender && map.Tag.Equals(tag));
        }

        private IReadOnlyList<Notification> GetPublished(Func<NotificationMap, bool> filter)
        {
            IReadOnlyList<Notification> notifications;
            lock (_listLock)
            {
                notifications = _published.Union(_pendingPubs).Where(filter)
                .Select(map => map.Notification)
                .ToArray();
            }

            return notifications;
        }

        /// <inheritdoc />
        public void Publish(INotificationSender sender, Notification notification)
        {
            Publish(sender, notification, new object());
        }

        /// <inheritdoc />
        public void Publish(INotificationSender sender, Notification notification, object tag)
        {
            if (string.IsNullOrEmpty(sender.Identifier))
                throw new InvalidOperationException("The identifier of the sender must be set");

            if (notification == null)
                throw new ArgumentNullException(nameof(notification), "Notification must be set");

            lock (_listLock)
            {
                // Lets check if the notification was already published
                var isPending = _pendingPubs.Union(_pendingAcks).Union(_published)
                    .Any(n => n.Notification == notification);

                if (isPending)
                    throw new InvalidOperationException("Notification cannot be published twice!");
                notification.Created = DateTime.Now;
                notification.Sender = sender.Identifier;

                _pendingPubs.Add(new NotificationMap(sender, notification, tag));
            }

            Published?.Invoke(this, notification);
        }

        /// <inheritdoc />
        public void Acknowledge(INotificationSender sender, Notification notification)
        {
            if (string.IsNullOrEmpty(sender.Identifier))
                throw new InvalidOperationException("The identifier of the sender must be set");

            if (notification == null)
                throw new ArgumentNullException(nameof(notification), "Notification must be set");

            notification.Acknowledged = DateTime.Now;
            notification.Acknowledger = sender.Identifier;

            NotificationMap published;
            lock (_listLock)
            {
                published = _published.SingleOrDefault(n => n.Notification.Identifier == notification.Identifier);
                if (published is not null)
                    _published.Remove(published);
                else
                {
                    published = _pendingPubs.SingleOrDefault(n => n.Notification.Identifier == notification.Identifier);

                    if (published is null)
                        throw new InvalidOperationException("Notification was not managed by the adapter. " +
                                                            "The sender was not registered on the adapter");

                    _pendingPubs.Remove(published);
                }

                _pendingAcks.Add(published);
            }

            if (published is not null)
                Acknowledged?.Invoke(this, published.Notification);
        }

        /// <inheritdoc />
        public void AcknowledgeAll(INotificationSender sender)
        {
            AcknowledgeByFilter(sender, map => map.Sender == sender);
        }

        /// <inheritdoc />
        public void AcknowledgeAll(INotificationSender sender, object tag)
        {
            AcknowledgeByFilter(sender, map => map.Sender == sender && Equals(map.Tag, tag));
        }

        /// <summary>
        /// Acknowledges notifications by a sender and given filter
        /// </summary>
        private void AcknowledgeByFilter(INotificationSender sender, Predicate<NotificationMap> filter)
        {
            NotificationMap[] publishes;
            lock (_listLock)
            {
                publishes = _published.Union(_pendingPubs).Where(m => filter(m)).ToArray();
                _published.RemoveAll(filter);
                _pendingPubs.RemoveAll(filter);

                foreach (var published in publishes)
                {
                    published.Notification.Acknowledged = DateTime.Now;
                    published.Notification.Acknowledger = sender.Identifier;

                    _pendingAcks.Add(published);
                }
            }

            foreach (var published in publishes ?? [])
                Acknowledged?.Invoke(this, published.Notification);
        }

        #endregion

        #region Facade <> Adapter

        /// <inheritdoc />
        IReadOnlyList<Notification> INotificationSourceAdapter.GetPublished()
        {
            IReadOnlyList<Notification> published;
            lock (_listLock)
            {
                published = _published.Union(_pendingAcks).Select(map => map.Notification).ToArray();
            }

            return published;
        }

        /// <inheritdoc />
        void INotificationSourceAdapter.Acknowledge(Notification notification)
        {
            NotificationMap map;
            lock (_listLock)
            {
                map = _published.Single(m => m.Notification.Identifier == notification.Identifier);
            }

            map.Sender.Acknowledge(map.Notification, map.Tag);
        }

        /// <inheritdoc />
        void INotificationSourceAdapter.AcknowledgeProcessed(Notification notification)
        {
            lock (_listLock)
            {
                var map = _pendingAcks.SingleOrDefault(n => n.Notification.Identifier.Equals(notification.Identifier));

                // Maybe already removed from this adapter
                if (map is not null)
                    _pendingAcks.Remove(map);
            }
        }

        /// <inheritdoc />
        void INotificationSourceAdapter.PublishProcessed(Notification notification)
        {
            NotificationMap map;
            lock (_listLock)
            {
                map = _pendingPubs.SingleOrDefault(n => n.Notification.Identifier.Equals(notification.Identifier));

                if (map != null)
                {
                    _pendingPubs.Remove(map);
                    _published.Add(map);
                    return;
                }
            }

            // If necessary we can acknowledge it
            if (notification.Acknowledged is null)
            {
                Logger.Log(LogLevel.Error, "Notification {0} was removed from the pending publications " +
                    "before being published but is not acknowledged.", notification.Identifier);
                notification.Acknowledged = DateTime.Now;
                notification.Acknowledger = nameof(NotificationAdapter);
                Acknowledged?.Invoke(this, notification);
            }

            Logger.Log(LogLevel.Warning, "Notification {0} was removed from the pending publications. " +
                "It was already acknowledged by {1} at {2}.", notification.Identifier, notification.Acknowledger, notification.Acknowledged);
        }

        /// <inheritdoc />
        void INotificationSourceAdapter.Sync()
        {
            // Publish pending notifications
            NotificationMap[] pendingPublishes = [];
            lock (_listLock)
            {
                pendingPublishes = _pendingPubs.ToArray();
            }
            foreach (var pendingPublish in pendingPublishes)
            {
                Published?.Invoke(this, pendingPublish.Notification);
            }

            // Acknowledge pending acknowledges
            NotificationMap[] pendingAcks = [];
            lock (_listLock)
            {
                pendingAcks = _pendingAcks.ToArray();
            }
            foreach (var pendingAck in pendingAcks)
            {
                Acknowledged?.Invoke(this, pendingAck.Notification);
            }
        }

        /// <inheritdoc />
        public event EventHandler<Notification> Published;

        /// <inheritdoc />
        public event EventHandler<Notification> Acknowledged;

        #endregion

        private class NotificationMap
        {
            public NotificationMap(INotificationSender sender, Notification notification, object tag)
            {
                Sender = sender;
                Notification = notification;
                Tag = tag;
            }

            public Notification Notification { get; }

            public INotificationSender Sender { get; }

            public object Tag { get; }
        }
    }
}
