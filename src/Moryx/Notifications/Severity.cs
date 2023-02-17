// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Notifications
{
    /// <summary>
    /// Different levels to differentiate the severity of a message
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// The notification is just for informational purpose.
        /// </summary>
        Info,

        /// <summary>
        /// The notification shows a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// The notification shows an error.
        /// </summary>
        Error,

        /// <summary>
        /// The notification shows an fatal error.
        /// </summary>
        Fatal
    }
}
