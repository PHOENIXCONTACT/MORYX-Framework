// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Notifications
{
    /// <summary>
    /// Standard implementation of INoficication
    /// </summary>
    public class Notification : INotification
    {
        /// <summary>
        /// Create new notification instance
        /// </summary>
        public Notification()
        {
            Identifier = new Guid();
        }

        /// <summary>
        /// Identifier of the notification
        /// </summary>
        public Guid Identifier { get; }

        public Severity Severity { get; set; }

        public DateTime Timestamp { get; set; }

        public string Sender { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public bool Confirmable { get; set; }

        public IReadOnlyList<object> Tags { get; set; }
    }
}