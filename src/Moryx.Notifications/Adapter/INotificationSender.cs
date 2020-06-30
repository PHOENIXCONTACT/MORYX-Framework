// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Notifications
{
    /// <summary>
    /// Interface for components which are sending notifications
    /// </summary>
    public interface INotificationSender
    {
        /// <summary>
        /// Name of the notification sender
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Inform the sender about the acknowledged notification
        /// </summary>
        void Acknowledge(INotification notification, object tag);
    }
}
