// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Notifications.Publisher;
using Moryx.TestTools.UnitTest;
using Moryx.Threading;
using Moq;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using NUnit.Framework;
using Moryx.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Notifications.Publisher.Model;

namespace Moryx.Notifications.Tests
{
    [TestFixture]
    public class NotificationPublisherTests
    {
        private NotificationManager _notificationManager;
        private Mock<INotificationProcessor> _processorMock;

        private Mock<INotificationSource> _notificationSource1Mock;
        private Mock<INotificationSource> _notificationSource2Mock;

        private const string SourceName1 = "TestSource";
        private const string SourceName2 = "TestSource2";

        private static readonly Guid TestIdentifier = Guid.NewGuid();

        private IUnitOfWorkFactory<NotificationsContext> _notificationModel;

        [SetUp]
        public void TestFixtureSetUp()
        {
        }

        [SetUp]
        public void SetUp()
        {
            _processorMock = new Mock<INotificationProcessor>();

            _notificationSource1Mock = new Mock<INotificationSource>();
            _notificationSource1Mock.SetupGet(s => s.Name).Returns(SourceName1);
            _notificationSource1Mock.SetupGet(s => s.IsActivated).Returns(true);
            _notificationSource1Mock.Setup(s => s.GetPublished()).Returns(Array.Empty<Notification>());

            _notificationSource2Mock = new Mock<INotificationSource>();
            _notificationSource2Mock.SetupGet(s => s.Name).Returns(SourceName2);
            _notificationSource2Mock.SetupGet(s => s.IsActivated).Returns(true);
            _notificationSource2Mock.Setup(s => s.GetPublished()).Returns(Array.Empty<Notification>());

            _notificationModel = new UnitOfWorkFactory<NotificationsContext>(new InMemoryDbContextManager(Guid.NewGuid().ToString()));

            _notificationManager = new NotificationManager
            {
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                ParallelOperations = new NotSoParallelOps(),
                Sources = [_notificationSource1Mock.Object, _notificationSource2Mock.Object],
                Processors = [_processorMock.Object],
                UnitOfWorkFactory = _notificationModel
            };
        }

        [TearDown]
        public void TearDown()
        {
            _notificationManager.Stop();
            _processorMock.Verify(p => p.Stop(), Times.Once,
                "There should be a stop call at the processor if the publish will be stopped");
        }

        [Test(Description = "There should be a restored notification map after the start of the publisher")]
        public void RestoreNotificationsFromActivatedSources()
        {
            // Arrange
            var notification = CreateTestNotification();
            _notificationSource1Mock.Setup(s => s.GetPublished()).Returns([notification]);

            // Act
            _notificationManager.Start();

            // Assert
            Assert.That(_notificationManager.GetAll().Length, Is.EqualTo(1), "There should be 1 mapped notification after the startup");
        }

        [Test(Description = "A acknowledge request should call an acknowledgement at the notification source")]
        public void AnAcknowledgeRequestShouldCallAnAcknowledgeAtTheNotificationSource()
        {
            // Arrange
            var notification = CreateTestNotification();
            _notificationSource1Mock.Setup(s => s.GetPublished()).Returns([notification]);
            _notificationManager.Start();

            // Act
            _notificationManager.Acknowledge(TestIdentifier);

            // Assert
            _notificationSource1Mock.Verify(n => n.Acknowledge(notification), Times.Once,
                "There should be an acknowledgement call at the notification source");
        }

        [Test(Description = "A new notification should be processed by a processor and the notification source should be informed as a confirmation. " +
                            "The publisher should also raise the publish event to inform the clients.")]
        public void APublishedNotificationShouldBeProcessedAndTheSourceShouldBeInformed()
        {
            // Arrange
            _notificationSource1Mock.Setup(s => s.GetPublished()).Returns(Array.Empty<Notification>());

            var notification = CreateTestNotification();
            _processorMock.Setup(p => p.CanProcess(notification)).Returns(true);
            bool raised = false;
            _notificationManager.Published += delegate { raised = true; };
            _notificationManager.Start();

            // Act
            _notificationSource1Mock.Raise(n => n.Published += null, _notificationSource1Mock.Object, notification);

            // Assert
            _processorMock.Verify(p => p.Process(notification), Times.Once,
                "There should be a processing of a new published notification");

            _notificationSource1Mock.Verify(n => n.PublishProcessed(notification), Times.Once,
                "There should be a confirmation that the notification was successfully published");

            Assert.That(raised, Is.True, "The publisher should raised the publish event if a notification was published by a notification source");
            Assert.That(_notificationManager.GetAll().Length, Is.EqualTo(1), "There should be a mapped notification");
        }

        [Test(Description = "An acknowledgement from the source should be processed by a processor and the notification source should be informed as a confirmation." +
                            "The publisher should also raise the acknowledged event to inform the clients.")]
        public void AnAcknowledgedNotificationShouldBeProcessedAndTheSourceShouldBeInformed()
        {
            // Arrange
            var notification = CreateTestNotification();
            _notificationSource1Mock.Setup(s => s.GetPublished()).Returns([notification]);

            _processorMock.Setup(p => p.CanProcess(notification)).Returns(true);
            bool raised = false;
            _notificationManager.Acknowledged += delegate { raised = true; };
            _notificationManager.Start();

            // Act
            _notificationSource1Mock.Raise(n => n.Acknowledged += null, _notificationSource1Mock.Object, notification);

            // Assert
            _processorMock.Verify(p => p.Acknowledge(notification), Times.Once,
                "There should be a processing of a acknowledged notification");

            _notificationSource1Mock.Verify(n => n.AcknowledgeProcessed(notification), Times.Once,
                "There should be a confirmation that the notification was successfully acknowledged");

            Assert.That(raised, Is.True,
                "The publisher should raise the acknowledged event of a notification was acknowledged by a notification source");
        }

        [Test(Description = "If a currently pending notification gets acknowledged, it should be acknowledged after publish.")]
        public void PendingNotificationShouldBeAcknowledgedAfterPublish()
        {
            // Arrange
            _notificationSource1Mock.Setup(s => s.GetPublished()).Returns(Array.Empty<Notification>());

            var notification = CreateTestNotification();
            _processorMock.Setup(p => p.CanProcess(notification)).Returns(true);

            var parallelOpsMock = new Mock<IParallelOperations>();
            Action<NotificationManager.NotificationMap> processAction = null;
            NotificationManager.NotificationMap processMap = null;

            parallelOpsMock.Setup(p => p.ExecuteParallel(It.IsAny<Action<NotificationManager.NotificationMap>>(),
                    It.IsAny<NotificationManager.NotificationMap>()))
                .Callback(delegate (Action<NotificationManager.NotificationMap> action,
                    NotificationManager.NotificationMap map)
                {
                    processAction = action;
                    processMap = map;
                });

            // Action of acknowledge can be synchronous
            parallelOpsMock.Setup(p => p.ExecuteParallel(It.IsAny<Action>())).Callback((Action ackAction) => ackAction());

            _notificationManager.ParallelOperations = parallelOpsMock.Object;
            _notificationManager.Start();

            // Act
            _notificationSource1Mock.Raise(n => n.Published += null, _notificationSource1Mock.Object, notification);

            // Acknowledge of manager should not be raised
            bool acknowledgedRaised = false;
            bool publishRaised = false;

            _notificationManager.Acknowledged += delegate { acknowledgedRaised = true; };
            _notificationManager.Published += delegate { publishRaised = true; };

            _notificationSource1Mock.Raise(n => n.Acknowledged += null, _notificationSource1Mock.Object, notification);

            // Assert
            Assert.That(acknowledgedRaised, Is.False);

            // Now finish processing
            processAction(processMap);

            // Now acknowledge should be success
            Assert.That(acknowledgedRaised, Is.True);
            Assert.That(publishRaised, Is.True);
        }

        private static Notification CreateTestNotification()
        {
            return CreateTestNotification(TestIdentifier);
        }

        private static Notification CreateTestNotification(Guid identifier, string sourceName = SourceName1)
        {
            var notification = new Notification();
            notification.Identifier = identifier;
            notification.Source = sourceName;
            return notification;
        }
    }
}

