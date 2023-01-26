// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Notifications
{
    /// <summary>
    /// Adapter for the <see cref="INotificationSource"/>
    /// </summary>
    public interface INotificationSourceAdapter
    {
        /// <summary>
        /// Returns the currently published notifications
        /// </summary>
        IReadOnlyList<Notification> GetPublished();

        /// <summary>
        /// Informs the sender of the notification to acknowledge it
        /// </summary>
        void Acknowledge(Notification notification);

        /// <summary>
        /// Restore notification on sender adapter
        /// </summary>
        void Sync();

        /// <summary>
        /// Informs a sender of an acknowledgement that it was processed
        /// </summary>
        void AcknowledgeProcessed(Notification notification);

        /// <summary>
        /// Informs a sender of an notification that it was processed
        /// </summary>
        void PublishProcessed(Notification notification);

        /// <summary>
        /// Event to publish a notification
        /// </summary>
        event EventHandler<Notification> Published;

        /// <summary>
        /// Event to publish an acknowledged notification
        /// </summary>
        event EventHandler<Notification> Acknowledged;
    }
}
