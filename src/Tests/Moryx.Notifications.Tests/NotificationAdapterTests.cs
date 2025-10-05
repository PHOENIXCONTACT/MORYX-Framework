// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moq;
using NUnit.Framework;

namespace Moryx.Notifications.Tests
{
    /// <summary>
    /// Unittests for <see cref="NotificationAdapter"/>
    /// </summary>
    [TestFixture]
    public class NotificationAdapterTests
    {
        private INotificationAdapter _adapter;
        private Mock<INotificationSender> _notificationSenderMock;
        private Notification _publishedEventNotification;
        private Notification _acknowledgedEventNotification;
        private Notification _acknowledgeCallNotification;
        private INotificationSender _sender;

        /// <summary>
        /// Initialize the test-environment
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _adapter = new NotificationAdapter();
            _notificationSenderMock = new Mock<INotificationSender>();
            _notificationSenderMock.Setup(n => n.Acknowledge(It.IsAny<Notification>(), It.IsAny<object>()))
                .Callback((Notification notification, object tag) => _acknowledgeCallNotification = notification);
            _notificationSenderMock.SetupGet(n => n.Identifier).Returns("Mock");
            _sender = _notificationSenderMock.Object;

            _publishedEventNotification = null;
            _acknowledgedEventNotification = null;
            _acknowledgeCallNotification = null;

            ((INotificationSourceAdapter)_adapter).Published += (sender, notification) =>
            {
                _publishedEventNotification = notification;
            };

            ((INotificationSourceAdapter)_adapter).Acknowledged += (sender, notification) =>
            {
                _acknowledgedEventNotification = notification;
            };
        }

        [Test(Description = "Check that publishing a notification publishes an event, and marks the notification as published. " +
                            "Check, that notifications can not be published twice.")]
        public void AdapterPublish()
        {
            // Arrange
            var notification = new Notification();

            // Act
            _adapter.Publish(_sender, notification);

            // Assert
            Assert.That(_publishedEventNotification, Is.Not.Null, "Published-event was not triggered.");
            Assert.That(_publishedEventNotification, Is.EqualTo(notification), "Published-event was triggered with wrong notification.");
            Assert.That(_publishedEventNotification.Identifier, Is.Not.Null, "Identifier should not be null.");
            Assert.That(_publishedEventNotification.Created, Is.Not.EqualTo(default(DateTime)), "Created date should have been set");

            Assert.Throws<InvalidOperationException>(delegate
            {
                _adapter.Publish(_sender, notification);
            }, "The same notification was published a second time.");
        }

        [Test(Description = "Publishes a notification with a tag")]
        public void AdapterPublishWithTag()
        {
            // Arrange
            var notification = new Notification();
            var tag = new object();

            // Act
            _adapter.Publish(_sender, notification, tag);
            ((INotificationSourceAdapter)_adapter).PublishProcessed(notification);

            // Arrange
            var published = _adapter.GetPublished(_sender, tag);
            Assert.That(published.Count, Is.EqualTo(1));
        }

        [Test(Description = "Check that acknowledging a notification by the adapter for a known notification.")]
        public void AdapterAcknowledgeForKnownNotification()
        {
            // Arrange
            var notification = new Notification();
            _adapter.Publish(_sender, notification);

            // Act
            _adapter.Acknowledge(_sender, notification);

            // Assert
            Assert.That(_acknowledgedEventNotification, Is.Not.Null, "Acknowledged-event was not triggered.");
            Assert.That(_acknowledgedEventNotification, Is.EqualTo(notification), "Acknowledged-event was triggered with wrong notification.");
            Assert.That(_acknowledgedEventNotification.Acknowledged, Is.Not.EqualTo(default(DateTime)), "Acknowledged date should have been set");
        }

        [Test(Description = "Check that acknowledging a notification by the adapter for a known notification.")]
        public void AdapterAcknowledgeKnownNotificationWhichIsAlreadyPublishedToThePublisher()
        {
            // Arrange
            var notification = new Notification();
            _adapter.Publish(_sender, notification);
            ((INotificationSourceAdapter)_adapter).PublishProcessed(notification);

            // Act
            _adapter.Acknowledge(_sender, notification);

            // Assert
            Assert.That(_acknowledgedEventNotification, Is.Not.Null, "Acknowledged-event was not triggered.");
            Assert.That(_acknowledgedEventNotification, Is.EqualTo(notification), "Acknowledged-event was triggered with wrong notification.");
            Assert.That(_acknowledgedEventNotification.Acknowledged, Is.Not.EqualTo(default(DateTime)), "Acknowledged date should have been set");
        }

        [Test(Description = "Check that acknowledging a notification by the adapter for a known notification throws an exception.")]
        public void AdapterAcknowledgeForUnknownNotification()
        {
            // Arrange
            var notification = new Notification();

            // Act && Assert
            Assert.Throws<InvalidOperationException>(delegate
            {
                _adapter.Acknowledge(_sender, notification);
            }, "Acknowledge was called for an unknown notification");
        }

        [Test(Description = "Check that acknowledging a notification by the SenderAdapter-interface " +
                            "for a known notification is delegated to the original sender.")]
        public void AcknowledgeForKnownNotification()
        {
            // Arrange
            var notification = new Notification();
            _adapter.Publish(_sender, notification);

            ((INotificationSourceAdapter)_adapter).PublishProcessed(notification);

            // Act
            ((INotificationSourceAdapter)_adapter).Acknowledge(notification);

            //Assert
            Assert.That(_acknowledgeCallNotification, Is.Not.Null, "Acknowledged was not called on the sender.");
            Assert.That(_acknowledgeCallNotification, Is.EqualTo(notification), "Acknowledged was not called for the wrong notification.");
        }

        /// <summary>
        /// Check that acknowledging a notification by the SenderAdapter-interface for an unknown notification throws an exception.
        /// </summary>
        [Test(Description = "Check that acknowledging a notification by the SenderAdapter-interface for an unknown notification throws an exception.")]
        public void SenderAdapterAcknowledgeForUnknownNotification()
        {
            // Arrange
            var notification = new Notification();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(delegate
            {
                ((INotificationSourceAdapter)_adapter).Acknowledge(notification);
            }, "Acknowledge was called for an unknown notification");
        }

        [Test(Description = "Pending published notifications should be published again during a sync because of a restart of the Publisher")]
        public void PublishPendingNotificationsDuringTheSync()
        {
            // Arrange
            _adapter.Publish(_sender, new Notification());
            _adapter.Publish(_sender, new Notification());
            _adapter.Publish(_sender, new Notification());
            _adapter.Publish(_sender, new Notification());
            int counter = 0;
            ((INotificationSourceAdapter) _adapter).Published += delegate { counter += 1; };

            // Act
            ((INotificationSourceAdapter) _adapter).Sync();

            // Assert
            Assert.That(counter, Is.EqualTo(4), "There should be four publish events. One for each pending notification");
        }

        [Test(Description = "Pending acknowledgements should be acknowledged again during a sync because of a restart of the Publisher")]
        public void AcknowledgePendingAcknowledgementsDuringTheSync()
        {
            // Arrange
            var notifiaction1 = new Notification();
            var notifiaction2 = new Notification();

            _adapter.Publish(_sender, notifiaction1);
            _adapter.Publish(_sender, notifiaction2);
            _adapter.Acknowledge(_sender, notifiaction1);
            _adapter.Acknowledge(_sender, notifiaction2);
            int counter = 0;
            ((INotificationSourceAdapter) _adapter).Acknowledged += delegate { counter += 1; };

            // Act
            ((INotificationSourceAdapter) _adapter).Sync();

            // Assert
            Assert.That(counter, Is.EqualTo(2), "There should be two ackowledge events. One for each pending acknowledgement which should be synchronized with the Publisher");
        }

        [Test(Description = "Nothing to do during the synchronization if everything is up to date")]
        public void NothingToDoIfEverythingIsUpToDate()
        {
            // Arrange
            var notification1 = new Notification();
            var notification2 = new Notification();
            _adapter.Publish(_sender, notification1);
            _adapter.Publish(_sender, notification2);
            ((INotificationSourceAdapter)_adapter).PublishProcessed(notification1);
            ((INotificationSourceAdapter)_adapter).PublishProcessed(notification2);
            int counter = 0;
            ((INotificationSourceAdapter) _adapter).Published += delegate { counter += 1; };

            // Act
            ((INotificationSourceAdapter) _adapter).Sync();

            // Assert
            Assert.That(counter, Is.EqualTo(0), "There should be no publish events because everything should be up to date");
        }
    }
}
