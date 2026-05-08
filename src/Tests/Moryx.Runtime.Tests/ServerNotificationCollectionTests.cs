// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.VisualBasic;
using Moryx.Modules;
using Moryx.Runtime.Modules;
using Moryx.Runtime.Tests.Mocks;
using NUnit.Framework;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace Moryx.Runtime.Tests
{
    [TestFixture]
    public class ServerNotificationCollectionTests
    {
        private const int MaxCollectionSize = 2500;
        private ServerNotificationCollection _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = [];
        }

        [Test]
        public void NotificationIsAdded()
        {
            var notification = new ModuleNotification(Notifications.Severity.Info, "notification", null);

            _sut.Add(notification);

            Assert.That(_sut.Single().Message, Is.EqualTo("notification")); 
        }

        [Test]
        public void NotificationCountDoesntExceedMaxSize()
        {
            var dummyNotification = new ModuleNotification(Notifications.Severity.Info, "dummy", null);

            for (int i = 0; i < MaxCollectionSize + 1; i++)
            {
                _sut.Add(dummyNotification);
            }
            Assert.That(_sut.Count, Is.EqualTo(MaxCollectionSize));
        }

        [Test]
        public void FirstItemGetsRemovedOnOverflow()
        {
            var firstNotification = new ModuleNotification(Notifications.Severity.Info, "first", null);
            var dummyNotification = new ModuleNotification(Notifications.Severity.Info, "dummy", null);

            _sut.Add(firstNotification);
            for (int i = 0; i < MaxCollectionSize; i++)
            {
                _sut.Add(dummyNotification);
            }

            Assert.That(_sut.First().Message, Is.EqualTo("dummy"));
        }

        [Test]
        public void CollectionChangedEventGetsInvokedForRemovedItem()
        {
            var firstNotification = new ModuleNotification(Notifications.Severity.Info, "first", null);
            var dummyNotification = new ModuleNotification(Notifications.Severity.Info, "dummy", null);
            ModuleNotification removedNotification = null;
            _sut.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    removedNotification = (ModuleNotification)e.OldItems[0];
                }
            };

            _sut.Add(firstNotification);
            for (int i = 0; i < MaxCollectionSize; i++)
            {
                _sut.Add(dummyNotification);
            }

            Assert.That(removedNotification.Message, Is.EqualTo(firstNotification.Message));
        }
    }
}
