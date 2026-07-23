// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Notifications.Publisher;

internal class NotificationPublisherFacade : FacadeBase, INotificationPublisher
{
    public INotificationManager NotificationManager { get; set; }

    public NotificationPublisherFacade()
    {
        ValidateHealthState = InitialValidateHealthState;
    }

    public override void Activate()
    {
        NotificationManager.Published += OnPublished;
        NotificationManager.Acknowledged += OnAcknowledged;
    }

    public override void Deactivate()
    {
        NotificationManager.Published -= OnPublished;
        NotificationManager.Acknowledged -= OnAcknowledged;
    }

    public Notification[] GetAll()
    {
        ValidateHealthState();
        return NotificationManager.GetAll();
    }

    private void OnPublished(object sender, Notification e)
    {
        Published?.Invoke(this, e);
    }

    private void OnAcknowledged(object sender, Notification e)
    {
        Acknowledged?.Invoke(this, e);
    }

    public event EventHandler<Notification> Published;

    public event EventHandler<Notification> Acknowledged;

    public Notification Get(Guid id)
    {
        return GetAll().Single(n => n.Identifier == id) as Notification;
    }

    public void Acknowledge(Notification notification)
    {
        NotificationManager.Acknowledge(notification.Identifier);
    }

    private void InitialValidateHealthState()
    {
        throw new HealthStateException(ServerModuleState.Stopped);
    }
}