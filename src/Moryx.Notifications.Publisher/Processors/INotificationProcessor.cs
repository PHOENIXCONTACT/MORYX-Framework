// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Notifications.Publisher
{
    /// <summary>
    /// Interface to be implemented by notification processors.
    /// </summary>
    public interface INotificationProcessor : IPlugin
    {
        /// <summary>
        /// Checks whether this instance is able to process notification of the given type.
        /// </summary>
        bool CanProcess(Notification notification);

        /// <summary>
        /// Checks whether this instance is able to process notification of the given type.
        /// </summary>
        bool CanProcess(Type type);

        /// <summary>
        /// Checks whether this instance is able to process notification of the given type.
        /// </summary>
        bool CanProcess(string type);

        /// <summary>
        /// Processes a new notification.
        /// </summary>
        NotificationProcessorResult Process(Notification notification);

        /// <summary>
        /// Notification was acknowledged
        /// </summary>
        NotificationProcessorResult Acknowledge(Notification notification);

        /// <summary>
        /// Loads with given parameters the preferred history of notifications
        /// </summary>
        IReadOnlyList<Notification> GetSelectedHistory(DateTime start, DateTime end, string title, Severity severity);

        /// <summary>
        /// Access to the processor for loading acknowledged notifications
        /// </summary>
        IReadOnlyList<Notification> GetHistory(DateTime start, DateTime end, Severity[] severity);
    }
}

