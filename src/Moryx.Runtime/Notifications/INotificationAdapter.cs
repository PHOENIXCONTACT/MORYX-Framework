// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.Notifications;

namespace Moryx.Runtime.Notifications
{
    /// <summary>
    /// Adapter within 
    /// </summary>
    public interface INotificationAdapter
    {
        /// <summary>
        /// Publish a notification within the system
        /// </summary>
        void Publish(INotificationSender sender, INotification notification);

        /// <summary>
        /// Acknowledge a published notification. The original sender will be informed
        /// and decides whether or not to remove the notification
        /// </summary>
        void Confirm(INotificationSender sender, INotification notification);

        /// <summary>
        /// Remove a published notification
        /// </summary>
        void Clear(INotificationSender sender, INotification notification);

        /// <summary>
        /// Remove a published notification
        /// </summary>
        void ClearAll(INotificationSender sender);

        /// <summary>
        /// Remove a published notification
        /// </summary>
        void ClearAll(INotificationSender sender, params object[] tags);

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