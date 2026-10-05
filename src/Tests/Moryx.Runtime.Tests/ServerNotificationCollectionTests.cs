// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Specialized;
using Moryx.Runtime.Modules;
using NUnit.Framework;

namespace Moryx.Runtime.Tests
{
    [TestFixture]
    public class ServerNotificationCollectionTests
    {
        // The agreed maximum size of the `ServerNotificationCollection` which
        // should provide an amount of notifications, that is large enough
        // without hitting memory limits
        private const int MaxCollectionSize = 2500;
        private ServerNotificationCollection _serverNotificationCollection;

        [SetUp]
        public void SetUp()
        {
            _serverNotificationCollection = [];
        }

        [Test]
        public void NotificationIsAdded()
        {
            var notification = new ModuleNotification(Notifications.Severity.Info, "notification", null);

            _serverNotificationCollection.Add(notification);

            Assert.That(_serverNotificationCollection.Single().Message, Is.EqualTo("notification"));
        }

        [Test]
        public void NotificationCountDoesntExceedMaxSize()
        {
            var dummyNotification = new ModuleNotification(Notifications.Severity.Info, "dummy", null);

            for (var i = 0; i < MaxCollectionSize + 1; i++)
            {
                _serverNotificationCollection.Add(dummyNotification);
            }
            Assert.That(_serverNotificationCollection.Count, Is.EqualTo(MaxCollectionSize));
        }

        [Test]
        public void FirstItemGetsRemovedOnOverflow()
        {
            var firstNotification = new ModuleNotification(Notifications.Severity.Info, "first", null);
            var dummyNotification = new ModuleNotification(Notifications.Severity.Info, "dummy", null);

            _serverNotificationCollection.Add(firstNotification);
            for (var i = 0; i < MaxCollectionSize; i++)
            {
                _serverNotificationCollection.Add(dummyNotification);
            }

            Assert.That(_serverNotificationCollection.First().Message, Is.EqualTo("dummy"));
        }

        [Test]
        public void CollectionChangedEventGetsInvokedForRemovedItem()
        {
            var firstNotification = new ModuleNotification(Notifications.Severity.Info, "first", null);
            var dummyNotification = new ModuleNotification(Notifications.Severity.Info, "dummy", null);
            ModuleNotification removedNotification = null;
            _serverNotificationCollection.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    removedNotification = (ModuleNotification)e.OldItems[0];
                }
            };

            _serverNotificationCollection.Add(firstNotification);
            for (int i = 0; i < MaxCollectionSize; i++)
            {
                _serverNotificationCollection.Add(dummyNotification);
            }

            Assert.That(removedNotification.Message, Is.EqualTo(firstNotification.Message));
        }
    }
}
