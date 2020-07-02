// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Notifications
{
    /// <summary>
    /// Common interface for different types of notifications
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// Unique identifier of this notification
        /// </summary>
        Guid Identifier { get; }

        /// <summary>
        /// Type of this notification
        /// </summary>
        Severity Severity { get; }
        
        /// <summary>
        /// Time stamp of occurence
        /// </summary>
        DateTime Timestamp { get; }
        
        /// <summary>
        /// Component or class which created and published the notification
        /// </summary>
        string Sender { get; }

        /// <summary>
        /// Optional title of this notification. Can be set by processor as well
        /// </summary>
        string Title { get; }
        
        /// <summary>
        /// Notification message
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Notification can be confirmed externally
        /// </summary>
        bool Confirmable { get; }

        /// <summary>
        /// Tags that can be used to group, identify or confirm notifications
        /// </summary>
        IReadOnlyList<object> Tags { get; }
    }
}