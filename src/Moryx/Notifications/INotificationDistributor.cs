// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Notifications
{
    /// <summary>
    /// Central kernel component that distributes within the system
    /// </summary>
    public interface INotificationDistributor
    {
        /// <summary>
        /// Publish a notification within the system
        /// </summary>
        void Publish(INotification notification, INotificationCertificate certificate);

        /// <summary>
        /// Acknowledge a published notification. The original sender will be informed
        /// and decides whether or not to remove the notification
        /// </summary>
        void Confirm(INotification notification, string confirmationIdentity);

        /// <summary>
        /// Remove a published notification
        /// </summary>
        void Clear(INotification notification, string signature);

        /// <summary>
        /// Get all notifications
        /// </summary>
        IReadOnlyList<INotification> GetAll();

        /// <summary>
        /// Get all notifications that contain the tags
        /// </summary>
        IReadOnlyList<INotification> GetAll(params object[] tags);

        /// <summary>
        /// Notification was published
        /// </summary>
        event EventHandler<INotification> Published;

        /// <summary>
        /// User or other module wants to acknowledge a notification
        /// </summary>
        event EventHandler<INotification> Confirmed;

        /// <summary>
        /// Notification was cleared and is no longer relevant
        /// </summary>
        event EventHandler<INotification> Cleared;
    }
}