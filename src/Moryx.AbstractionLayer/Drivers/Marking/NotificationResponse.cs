// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Response for different messages which can be used for example by the notification bar like error messages or warnings
    /// </summary>
    public class NotificationResponse : TransmissionResult
    {
        /// <summary>
        /// Specific number of error
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// description of error
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}
