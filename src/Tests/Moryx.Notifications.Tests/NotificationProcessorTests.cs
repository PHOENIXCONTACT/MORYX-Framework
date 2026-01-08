// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Notifications.Publisher;
using Moryx.Notifications.Publisher.Model;
using NUnit.Framework;

namespace Moryx.Notifications.Tests
{
    [TestFixture]
    public class NotificationProcessorTests
    {
        private NotificationProcessor _processor;
        private IUnitOfWorkFactory<NotificationsContext> _unitOfWorkFactory;

        [SetUp]
        public void TestFixtureSetUp()
        {
            _unitOfWorkFactory = new UnitOfWorkFactory<NotificationsContext>(new InMemoryDbContextManager(Guid.NewGuid().ToString()));
            _processor = new NotificationProcessor()
            {
                UnitOfWorkFactory = _unitOfWorkFactory
            };
        }

        [SetUp]
        public void SetUp()
        {
            _processor.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _processor.Stop();
            using (var uow = _unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<INotificationEntityRepository>();
                var entities = repo.GetAll();
                repo.RemoveRange(entities);
                var typeRepo = uow.GetRepository<INotificationTypeEntityRepository>();
                var typeEntities = typeRepo.GetAll();
                typeRepo.RemoveRange(typeEntities);
                uow.SaveChanges();
            }
        }

        [Test(Description = "A processed notification should be stored in the database")]
        public void ProcessedNotificationsShouldBeStored()
        {
            // Arrange
            var managed = CreateTestNotification();

            // Act
            _processor.Process(managed);

            // Assert
            using (var uow = _unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<INotificationEntityRepository>();
                var entities = repo.GetAll();
                Assert.That(entities.Count, Is.EqualTo(1), "There should be one entry");
                var entity = entities.First();
                Assert.That(entity.Identifier, Is.EqualTo(managed.Identifier), "There was a wrong identifier saved");
                Assert.That(entity.Sender, Is.EqualTo(managed.Sender), "There was a wrong Sender saved");
                Assert.That(entity.Source, Is.EqualTo(managed.Source), "There was a wrong Source saved");
                Assert.That(entity.Message, Is.EqualTo(managed.Message), "There was a wrong Message saved");
                Assert.That(entity.Title, Is.EqualTo(managed.Title), "There was a wrong Title saved");
                Assert.That(entity.Created.Kind, Is.EqualTo(DateTimeKind.Utc), "DateTime is not in UTC format");
            }
        }

        [Test(Description = "A processed notification which is already known should be updated in the database")]
        public void ProcessedKnownNotificationsShouldBeUpdated()
        {
            // Arrange
            var managed = CreateTestNotification();

            _processor.Process(managed);

            // Act
            managed.Message = "Updated Message";
            _processor.Process(managed);

            // Assert
            using (var uow = _unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<INotificationEntityRepository>();
                var entities = repo.GetAll();
                Assert.That(entities.Count, Is.EqualTo(1), "There should be one entry after the update");
                var entity = entities.First();
                Assert.That(entity.Identifier, Is.EqualTo(managed.Identifier), "There was a wrong identifier saved");
                Assert.That(entity.Sender, Is.EqualTo(managed.Sender), "There was a wrong Sender saved");
                Assert.That(entity.Source, Is.EqualTo(managed.Source), "There was a wrong Source saved");
                Assert.That(entity.Message, Is.EqualTo(managed.Message), "There was a wrong Message saved. It should be the updated one.");
                Assert.That(entity.Title, Is.EqualTo(managed.Title), "There was a wrong Title saved");
                Assert.That(_processor.GetTypes().Any(t => t.Id == 0), Is.False, "Id of type was not set correctly and is still 0");
            }
        }

        [Test(Description = "An acknowledgement should be saved at the notification entity")]
        public void AcknowledgedNotificationShouldBeSaved()
        {
            // Arrange
            var notification = CreateTestNotification();
            _processor.Process(notification);
            notification.Acknowledged = DateTime.Now;

            // Act
            _processor.Acknowledge(notification);

            // Assert
            using (var uow = _unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<INotificationEntityRepository>();
                var entities = repo.GetAll();
                Assert.That(entities.Count, Is.EqualTo(1), "There should only one entry after the acknowledgement");
                var entity = entities.First();
                Assert.That(entity.Acknowledged, Is.Not.Null, "There should be a saved acknowledge datetime");
                Assert.That(entity.Acknowledged?.Kind, Is.EqualTo(DateTimeKind.Utc), "DateTime is not in UTC format");
            }
        }

        [Test(Description = "A notification type should be reused if it is already known")]
        public void ReuseKnownTypesForNotifications()
        {
            // Arrange
            using (var uow = _unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<INotificationTypeEntityRepository>();
                repo.Create("Notification", 0); // Type: Notification, Severity: Info
                uow.SaveChanges();
            }
            var notification = CreateTestNotification();
            _processor.Start();

            // Act
            _processor.Process(notification);

            // Assert
            using (var uow = _unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<INotificationTypeEntityRepository>();
                var entities = repo.GetAll();
                Assert.That(entities.Count, Is.EqualTo(1), "There should only one type entity");
            }
        }

        private static Notification CreateTestNotification()
        {
            return new Notification
            {
                Severity = Severity.Info,
                Sender = "Sender",
                Source = "Source",
                Message = "Message",
                Title = "Title",
                Created = DateTime.UtcNow
            };
        }
    }
}

