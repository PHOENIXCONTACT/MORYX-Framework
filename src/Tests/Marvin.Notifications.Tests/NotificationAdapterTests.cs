using System;
using Moq;
using NUnit.Framework;

namespace Marvin.Notifications.Tests
{
    /// <summary>
    /// Unittests for <see cref="NotificationAdapter"/>
    /// </summary>
    [TestFixture]
    public class NotificationAdapterTests
    {
        private INotificationAdapter _adapter;
        private Mock<INotificationSender> _notificationSenderMock;
        private IManagedNotification _publishedEventNotification;
        private IManagedNotification _acknowledgedEventNotification;
        private INotification _acknowledgeCallNotification;
        private INotificationSender _sender;

        /// <summary>
        /// Initialize the test-environment
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _adapter = new NotificationAdapter();
            _notificationSenderMock = new Mock<INotificationSender>();
            _notificationSenderMock.Setup(n => n.Acknowledge(It.IsAny<INotification>(), It.IsAny<object>()))
                .Callback((INotification notification) => _acknowledgeCallNotification = notification);
            _notificationSenderMock.SetupGet(n => n.Identifier).Returns("Mock");
            _sender = _notificationSenderMock.Object;

            _publishedEventNotification = null;
            _acknowledgedEventNotification = null;
            _acknowledgeCallNotification = null;

            ((INotificationSourceAdapter)_adapter).Published += (sender, notification) =>
            {
                _publishedEventNotification = (IManagedNotification)notification;
            };

            ((INotificationSourceAdapter)_adapter).Acknowledged += (sender, notification) =>
            {
                _acknowledgedEventNotification = (IManagedNotification)notification;
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
            Assert.NotNull(_publishedEventNotification, "Published-event was not triggered.");
            Assert.AreEqual(notification, _publishedEventNotification, "Published-event was triggered with wrong notification.");
            Assert.NotNull(_publishedEventNotification.Identifier, "Identifier should not be null.");
            Assert.AreNotEqual(_publishedEventNotification.Created, default(DateTime), "Created date should have been set");

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
            Assert.AreEqual(1, published.Count);
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
            Assert.NotNull(_acknowledgedEventNotification, "Acknowledged-event was not triggered.");
            Assert.AreEqual(notification, _acknowledgedEventNotification, "Acknowledged-event was triggered with wrong notification.");
            Assert.AreNotEqual(_acknowledgedEventNotification.Acknowledged, default(DateTime), "Acknowledged date should have been set");
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
            Assert.NotNull(_acknowledgedEventNotification, "Acknowledged-event was not triggered.");
            Assert.AreEqual(notification, _acknowledgedEventNotification, "Acknowledged-event was triggered with wrong notification.");
            Assert.AreNotEqual(_acknowledgedEventNotification.Acknowledged, default(DateTime), "Acknowledged date should have been set");
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
            Assert.NotNull(_acknowledgeCallNotification, "Acknowledged was not called on the sender.");
            Assert.AreEqual(notification, _acknowledgeCallNotification, "Acknowledged was not called for the wrong notification.");
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
            Assert.AreEqual(4, counter, "There should be four publish events. One for each pending notification");
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
            Assert.AreEqual(2, counter, "There should be two ackowledge events. One for each pending acknowledgement which should be synchronized with the Publisher");
        }

        [Test(Description = "Nothing to do during the synchronization if everything is up to date")]
        public void NothingToDoIfEverythingIsUpToDate()
        {
            // Arrange
            var notifiaction1 = new Notification();
            var notification2 = new Notification();
            _adapter.Publish(_sender, notifiaction1);
            _adapter.Publish(_sender, notification2);
            ((INotificationSourceAdapter)_adapter).PublishProcessed(notifiaction1);
            ((INotificationSourceAdapter)_adapter).PublishProcessed(notification2);
            int counter = 0;
            ((INotificationSourceAdapter) _adapter).Published += delegate { counter += 1; };

            // Act
            ((INotificationSourceAdapter) _adapter).Sync();

            // Assert
            Assert.AreEqual(0, counter, "There should be no publish events because everything should be up to date");
        }
    }
}