// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Moq;
using Moryx.Notifications;
using Moryx.Orders.Management.Notifications;
using Moryx.Orders.Management.Properties;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests;

[TestFixture]
public class OperationNotificationsTests
{
    private OperationNotifications _operationNotifications;
    private OperationNotificationConfig _config;
    private Mock<INotificationAdapter> _notificationAdapterMock;
    private Mock<IOperationDataPool> _operationDataPoolMock;

    [SetUp]
    public void Setup()
    {
        var moduleConfig = new ModuleConfig();

        _config = new OperationNotificationConfig();
        moduleConfig.Notifications = _config;

        _notificationAdapterMock = new Mock<INotificationAdapter>();
        _operationDataPoolMock = new Mock<IOperationDataPool>();

        _operationNotifications = new OperationNotifications(moduleConfig, _notificationAdapterMock.Object, _operationDataPoolMock.Object);
    }

    [Test(Description = "Notification will be raised if operation changes to Amount-Reached")]
    public void NotifyIfOperationChangesToAmountReached()
    {
        // Arrange
        _config.EnableAmountReachedNotification = true;

        // No operations in pool at start
        _operationDataPoolMock.Setup(p => p.GetAll(It.IsAny<Func<IOperationData, bool>>()))
            .Returns([]);

        // No published notifications at start
        _notificationAdapterMock.Setup(p => p.GetPublished(It.IsAny<INotificationSender>(), It.IsAny<object>()))
            .Returns([]);

        _operationNotifications.Start();

        // Act
        var amountReachedOperationMock = PrepareOperationDataMock("order", "operation", true);
        _operationDataPoolMock.Raise(p => p.OperationUpdated += null,
            _operationDataPoolMock.Object, new OperationEventArgs(amountReachedOperationMock.Object));

        // Assert
        _notificationAdapterMock.Verify(n => n.Publish(
            _operationNotifications, It.IsAny<Notification>(), It.IsAny<object>()), Times.Once);
    }

    [Test(Description = "Notification will be acknowledged if operation changes from Amount-Reached to not Amount-Reached")]
    public void AcknowledgeAllIfNotAmountReachedAnymore()
    {
        // Arrange
        _config.EnableAmountReachedNotification = true;

        var operationMock = PrepareOperationDataMock("order", "operation", true);
        _operationDataPoolMock.Setup(p => p.GetAll(It.IsAny<Func<IOperationData, bool>>()))
            .Returns([operationMock.Object]);

        _operationNotifications.Start();

        var publishedNotification = new Notification
        {
            Message = "Test Notification"
        };
        _notificationAdapterMock.Setup(p => p.GetPublished(It.IsAny<INotificationSender>(), It.IsAny<object>()))
            .Returns([publishedNotification]);

        // Act
        var notAmountReachedStateMock = new Mock<IOperationState>();
        notAmountReachedStateMock.SetupGet(s => s.IsAmountReached).Returns(false);
        operationMock.SetupGet(o => o.State).Returns(notAmountReachedStateMock.Object);

        _operationDataPoolMock.Raise(p => p.OperationUpdated += null,
            _operationDataPoolMock.Object, new OperationEventArgs(operationMock.Object));

        // Assert
        _notificationAdapterMock.Verify(n => n.AcknowledgeAll(
            _operationNotifications, It.IsAny<object>()), Times.Once);
    }

    [Test(Description = "Publish Amount-Reached Notification on Start if enabled in config for all operations which already reached the amount")]
    [TestCase(false, 0, true, Description = "No operation in pool reached amount")]
    [TestCase(true, 1, true, Description = "One Operation in pool reached amount")]
    [TestCase(false, 0, false, Description = "No operation in pool reached amount, notification disabled")]
    [TestCase(true, 0, false, Description = "One Operation in pool reached amount, notification disabled")]
    public void PublishAmountReachedNotificationOnStart(bool amountReachedInPool, int expectedNotificationCount, bool notificationEnabled)
    {
        // Arrange
        const string orderNumber = "order";
        const string operationNumber = "operation";
        _config.EnableAmountReachedNotification = notificationEnabled;

        // Prepare operations in pool
        var amountReachedOperationMock = PrepareOperationDataMock(orderNumber, operationNumber, true);
        var otherOperationMock = PrepareOperationDataMock("other-order", "other-operation", false);

        List<IOperationData> operations = [otherOperationMock.Object];

        if (amountReachedInPool)
            operations.Add(amountReachedOperationMock.Object);

        _operationDataPoolMock.Setup(p => p.GetAll(It.IsAny<Func<IOperationData, bool>>()))
            .Returns((Func<IOperationData, bool> filter) => operations.Where(filter).ToArray());

        // Prepare notification-adapter
        Notification publishedNotification = null;
        _notificationAdapterMock.Setup(n => n.Publish(_operationNotifications,
            It.IsAny<Notification>(), It.IsAny<object>()))
            .Callback((INotificationSender _, Notification notification, object _) => publishedNotification = notification);

        // Act
        _operationNotifications.Start();

        // Assert
        _notificationAdapterMock.Verify(n => n.Publish(
            _operationNotifications, It.IsAny<Notification>(), It.IsAny<object>()), Times.Exactly(expectedNotificationCount));

        // If notification is expected, verify content
        if (amountReachedInPool && notificationEnabled)
        {
            var expectedNotification = string.Format(CultureInfo.CurrentCulture, Strings.OperationNotifications_AmountReachedNotificationMessage,
                $"{orderNumber}-{operationNumber}");

            Assert.That(publishedNotification.Message, Is.EqualTo(expectedNotification));
        }
        else
        {
            Assert.That(publishedNotification, Is.Null);
        }
    }

    private static Mock<IOperationData> PrepareOperationDataMock(string orderNumber, string operationNumber, bool isAmountReached)
    {
        var operationDataStateMock = new Mock<IOperationState>();
        operationDataStateMock.SetupGet(s => s.IsAmountReached).Returns(isAmountReached);

        var orderDataMock = new Mock<IOrderData>();
        orderDataMock.SetupGet(o => o.Number).Returns(orderNumber);

        var operationDataMock = new Mock<IOperationData>();
        operationDataMock.SetupGet(o => o.Identifier).Returns(Guid.NewGuid);
        operationDataMock.SetupGet(o => o.OrderData).Returns(orderDataMock.Object);
        operationDataMock.SetupGet(o => o.Number).Returns(operationNumber);
        operationDataMock.SetupGet(o => o.State).Returns(operationDataStateMock.Object);
        return operationDataMock;
    }
}
