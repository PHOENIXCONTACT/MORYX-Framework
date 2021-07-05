// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Notifications
{
    /// <summary>
    /// Interface for components that send/publish notifications
    /// </summary>
    public interface INotificationSender
    {
        /// <summary>
        /// Name of the sender
        /// </summary>
        string Name { get; }

        /// <summary>
        /// (Try to) confirm a notification 
        /// </summary>
        void Confirm(INotification notification);
    }
}