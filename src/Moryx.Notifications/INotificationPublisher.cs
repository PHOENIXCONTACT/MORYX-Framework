// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Notifications;

/// <summary>
/// Notification publisher facade
/// </summary>
public interface INotificationPublisher
{
    /// <summary>
    /// Returns all active notifications
    /// </summary>
    Notification[] GetAll();

    /// <summary>
    /// Gets a <see cref="Notification"/> corresponding to a specific <paramref name="id"/>
    /// If this is an inactive <see cref="Notification"/>, than the NotificationHistory will be used to search for the asked <see cref="Notification"/>
    /// </summary>
    /// <param name="id">Id of the desired <see cref="Notification"/></param>
    /// <returns>A <see cref="Notification"/> correspodning to the given id if it exists; null otherwise</returns>
    Notification Get(Guid id);

    /// <summary>
    /// Acknowledge the given Notificatione
    /// </summary>
    /// <param name="notification">The notification to be acknowledged</param>
    void Acknowledge(Notification notification);

    /// <summary>
    /// Raised if notification was published
    /// </summary>
    event EventHandler<Notification> Published;

    /// <summary>
    /// Raised if notification was acknowledged
    /// </summary>
    event EventHandler<Notification> Acknowledged;
}