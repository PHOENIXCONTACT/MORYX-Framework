// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Notifications
{
    /// <summary>
    /// Adapter for <see cref="INotificationSender"/> to publish and acknowledge notifications
    /// </summary>
    public interface INotificationAdapter
    {
        /// <summary>
        /// Will return currently published notifications
        /// </summary>
        IReadOnlyList<INotification> GetPublished(INotificationSender sender);

        /// <summary>
        /// Will return currently published notifications filtered by the given tag
        /// </summary>
        IReadOnlyList<INotification> GetPublished(INotificationSender sender, object tag);

        /// <summary>
        /// Publishes the given notification
        /// </summary>
        void Publish(INotificationSender sender, INotification notification);

        /// <summary>
        /// Publishes the given notification
        /// </summary>
        void Publish(INotificationSender sender, INotification notification, object tag);

        /// <summary>
        /// Acknowledges the given notification
        /// </summary>
        void Acknowledge(INotificationSender sender, INotification notification);

        /// <summary>
        /// Acknowledges all notifications of the given sender
        /// </summary>
        void AcknowledgeAll(INotificationSender sender);

        /// <summary>
        /// Acknowledges all notifications of the given sender and filtered by the given tag
        /// </summary>
        void AcknowledgeAll(INotificationSender sender, object tag);
    }
}
