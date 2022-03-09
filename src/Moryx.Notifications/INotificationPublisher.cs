// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Notifications
{
    /// <summary>
    /// Notification publisher facade
    /// </summary>
    [Obsolete]
    public interface INotificationPublisher
    {
        /// <summary>
        /// Returns all current notifications
        /// </summary>
        INotification[] GetAll();

        /// <summary>
        /// Raised if notification was published
        /// </summary>
        event EventHandler<INotification> Published;

        /// <summary>
        /// Raised if notification was acknowledged
        /// </summary>
        event EventHandler<INotification> Acknowledged;
    }
}
