// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Notifications.Publisher
{
    /// <summary>
    /// Result of the processing and acknowledge of the notification processor
    /// </summary>
    public enum NotificationProcessorResult
    {
        /// <summary>
        /// Notification was processed successfully
        /// </summary>
        Processed,

        /// <summary>
        /// Notification was ignored by the processor
        /// </summary>
        Ignored
    }
}
