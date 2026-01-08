// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Notifications.Publisher;

/// <summary>
/// Interface for the notification publisher base component
/// </summary>
internal interface INotificationManager : IInitializablePlugin
{
    /// <summary>
    /// Returns all current notifications
    /// </summary>
    Notification[] GetAll();

    /// <summary>
    /// Acknowledges an notification with the given identifier
    /// </summary>
    void Acknowledge(Guid identifier);

    /// <summary>
    /// Raised if notification was published
    /// </summary>
    event EventHandler<Notification> Published;

    /// <summary>
    /// Raised if notification was acknowledged
    /// </summary>
    event EventHandler<Notification> Acknowledged;
}