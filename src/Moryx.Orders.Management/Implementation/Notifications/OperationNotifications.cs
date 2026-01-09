// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Moryx.Container;
using Moryx.Notifications;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management.Notifications;

[Component(LifeCycle.Singleton, typeof(IOperationNotifications))]
internal class OperationNotifications : IOperationNotifications, INotificationSender
{
    private readonly INotificationAdapter _notificationAdapter;
    private readonly IOperationDataPool _operationDataPool;
    private readonly OperationNotificationConfig _config;

    public OperationNotifications(ModuleConfig moduleConfig, INotificationAdapter notificationAdapter, IOperationDataPool operationDataPool)
    {
        _config = moduleConfig.Notifications;
        _notificationAdapter = notificationAdapter;
        _operationDataPool = operationDataPool;
    }

    public void Start()
    {
        _operationDataPool.OperationUpdated += OnOperationUpdated;

        if (_config.EnableAmountReachedNotification)
        {
            var operationsReachedAmount = _operationDataPool.GetAll(o => o.State.IsAmountReached);
            foreach (var operationData in operationsReachedAmount)
            {
                PublishNewAmountReachedNotification(operationData);
            }
        }
    }

    public void Stop()
    {
        _operationDataPool.OperationUpdated -= OnOperationUpdated;

        _notificationAdapter.AcknowledgeAll(this);
    }

    private void OnOperationUpdated(object sender, OperationEventArgs e)
    {
        if (_config.EnableAmountReachedNotification)
        {
            NotifyAmountReachedNotification(e.OperationData);
        }
    }

    private void NotifyAmountReachedNotification(IOperationData operationData)
    {
        var tag = new OperationTag(operationData.Identifier, NotificationType.AmountReached);
        var notifications = _notificationAdapter.GetPublished(this, tag);

        if (notifications.Count > 0 && !operationData.State.IsAmountReached)
        {
            // Acknowledge existing notification
            _notificationAdapter.AcknowledgeAll(this, tag);
        }
        else if (notifications.Count == 0 && operationData.State.IsAmountReached)
        {
            // Publish new notification
            PublishNewAmountReachedNotification(operationData);
        }
    }

    private void PublishNewAmountReachedNotification(IOperationData operationData)
    {
        _notificationAdapter.Publish(this, new Notification
        {
            Title = Strings.OperationNotifications_AmountReachedNotificationTitle,
            Message = string.Format(CultureInfo.CurrentCulture, Strings.OperationNotifications_AmountReachedNotificationMessage,
                $"{operationData.OrderData.Number}-{operationData.Number}"),
            Severity = Severity.Info,
            IsAcknowledgable = false
        }, new OperationTag(operationData.Identifier, NotificationType.AmountReached));
    }

    string INotificationSender.Identifier => ModuleController.ModuleName;

    void INotificationSender.Acknowledge(Notification notification, object tag)
    {
        _notificationAdapter.Acknowledge(this, notification);
    }

    private readonly record struct OperationTag(Guid OperationId, NotificationType NotificationType);

    private enum NotificationType
    {
        AmountReached = 0
    }
}
