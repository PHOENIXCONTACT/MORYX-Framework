// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Marvin.Notifications
{
    /// <summary>
    /// Notification adapter for the server module.
    /// The events and calls will be redirected to the <see cref="INotificationSource"/>
    /// </summary>
    public class NotificationAdapter : INotificationAdapter, INotificationSourceAdapter
    {
        private readonly List<NotificationMap> _published = new List<NotificationMap>();
        private readonly List<NotificationMap> _pendingAcks = new List<NotificationMap>();
        private readonly List<NotificationMap> _pendingPubs = new List<NotificationMap>();

        private readonly ReaderWriterLockSlim _listLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #region Adapter <> Facade

        /// <inheritdoc />
        public IReadOnlyList<INotification> GetPublished(INotificationSender sender)
        {
            return GetPublished(map => map.Sender == sender);
        }

        /// <inheritdoc />
        public IReadOnlyList<INotification> GetPublished(INotificationSender sender, object tag)
        {
            return GetPublished(map => map.Sender == sender && map.Tag.Equals(tag));
        }

        private IReadOnlyList<INotification> GetPublished(Func<NotificationMap, bool> filter)
        {
            _listLock.EnterReadLock();

            var notifications = _published.Where(filter)
                .Select(map => map.Notification)
                .ToArray();

            _listLock.ExitReadLock();

            return notifications;
        }

        /// <inheritdoc />
        public void Publish(INotificationSender sender, INotification notification)
        {
            Publish(sender, notification, new object());
        }

        /// <inheritdoc />
        public void Publish(INotificationSender sender, INotification notification, object tag)
        {
            if (string.IsNullOrEmpty(sender.Identifier))
                throw new InvalidOperationException("The identifier of the sender must be set");

            if (notification == null)
                throw new ArgumentNullException(nameof(notification), "Notification must be set");

            var managed = (IManagedNotification)notification;
            managed.Identifier = Guid.NewGuid();
            managed.Created = DateTime.Now;
            managed.Sender = sender.Identifier;

            _listLock.EnterUpgradeableReadLock();

            // Lets check if the notification was already published
            var isPending = _pendingPubs.Union(_pendingAcks).Union(_published)
                .Any(n => n.Notification.Identifier.Equals(notification.Identifier));

            if (isPending)
            {
                _listLock.ExitUpgradeableReadLock();
                throw new InvalidOperationException("Notification cannot be published twice!");
            }

            _listLock.EnterWriteLock();

            _pendingPubs.Add(new NotificationMap(sender, notification, tag));

            _listLock.ExitWriteLock();
            _listLock.ExitUpgradeableReadLock();

            Published?.Invoke(this, notification);
        }

        /// <inheritdoc />
        public void Acknowledge(INotificationSender sender, INotification notification)
        {
            if (string.IsNullOrEmpty(sender.Identifier))
                throw new InvalidOperationException("The identifier of the sender must be set");

            if (notification == null)
                throw new ArgumentNullException(nameof(notification), "Notification must be set");

            var managed = (IManagedNotification)notification;
            managed.Acknowledged = DateTime.Now;
            managed.Acknowledger = sender.Identifier;

            _listLock.EnterWriteLock();

            var published = _published.SingleOrDefault(n => n.Notification.Identifier == notification.Identifier);
            if (published != null)
            {
                _published.Remove(published);
            }

            if (published == null)
            {
                published = _pendingPubs.SingleOrDefault(n => n.Notification.Identifier == notification.Identifier);

                if (published == null)
                {
                    _listLock.ExitWriteLock();
                    throw new InvalidOperationException("Notification was not managed by the adapter. " +
                                                        "The sender was not registered on the adapter");
                }

                _pendingPubs.Remove(published);
            }

            _pendingAcks.Add(published);

            _listLock.ExitWriteLock();

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
            _listLock.EnterWriteLock();

            var publishes = _published.Where(m => filter(m)).ToArray();
            _published.RemoveAll(filter);
            _pendingPubs.RemoveAll(filter);

            foreach (var published in publishes)
            {
                var managed = (IManagedNotification)published.Notification;
                managed.Acknowledged = DateTime.Now;
                managed.Acknowledger = sender.Identifier;

                _pendingAcks.Add(published);
            }

            _listLock.ExitWriteLock();

            foreach (var published in publishes)
                Acknowledged?.Invoke(this, published.Notification);
        }

        #endregion

        #region Facade <> Adapter

        /// <inheritdoc />
        IReadOnlyList<INotification> INotificationSourceAdapter.GetPublished()
        {
            _listLock.EnterReadLock();

            var published = _published.Union(_pendingAcks).Select(map => map.Notification).ToArray();

            _listLock.ExitReadLock();

            return published;
        }

        /// <inheritdoc />
        void INotificationSourceAdapter.Acknowledge(INotification notification)
        {
            _listLock.EnterReadLock();

            var map = _published.Single(m => m.Notification.Identifier == notification.Identifier);

            _listLock.ExitReadLock();

            map.Sender.Acknowledge(map.Notification, map.Tag);
        }

        /// <inheritdoc />
        void INotificationSourceAdapter.AcknowledgeProcessed(INotification notification)
        {
            _listLock.EnterWriteLock();

            var map = _pendingAcks.SingleOrDefault(n => n.Notification.Identifier.Equals(notification.Identifier));

            // Maybe already removed from this adapter
            if (map != null)
                _pendingAcks.Remove(map);

            _listLock.ExitWriteLock();
        }

        /// <inheritdoc />
        void INotificationSourceAdapter.PublishProcessed(INotification notification)
        {
            _listLock.EnterWriteLock();

            var map = _pendingPubs.SingleOrDefault(n => n.Notification.Identifier.Equals(notification.Identifier));

            if (map != null)
            {
                _pendingPubs.Remove(map);
                _published.Add(map);
                _listLock.ExitWriteLock();
            }
            else
            {
                // Notification is maybe not pending anymore - we only can acknowledge it
                _listLock.ExitWriteLock();

                var managed = (IManagedNotification)notification;
                managed.Acknowledged = DateTime.Now;
                managed.Acknowledger = nameof(NotificationAdapter);
                Acknowledged?.Invoke(this, notification);
            }
        }

        /// <inheritdoc />
        void INotificationSourceAdapter.Sync()
        {
            // Publish pending notifications
            _listLock.EnterReadLock();
            var pendingPublishs = _pendingPubs.ToArray();
            _listLock.ExitReadLock();

            foreach (var pendingPublish in pendingPublishs)
            {
                Published?.Invoke(this, pendingPublish.Notification);
            }

            // Acknowledge pending acknowledges
            _listLock.EnterReadLock();
            var pendingAcks = _pendingAcks.ToArray();
            _listLock.ExitReadLock();

            foreach (var pendingAck in pendingAcks)
            {
                Acknowledged?.Invoke(this, pendingAck.Notification);
            }
        }

        /// <inheritdoc />
        public event EventHandler<INotification> Published;

        /// <inheritdoc />
        public event EventHandler<INotification> Acknowledged;

        #endregion

        private class NotificationMap
        {
            public NotificationMap(INotificationSender sender, INotification notification, object tag)
            {
                Sender = sender;
                Notification = notification;
                Tag = tag;
            }

            public INotification Notification { get; }

            public INotificationSender Sender { get; }

            public object Tag { get; }
        }
    }
}
