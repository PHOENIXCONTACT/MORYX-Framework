// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Notifications.Publisher
{
    /// <summary>
    /// API for notification types. Will represent the fix data structure
    /// </summary>
    public interface INotificationType : IPersistentObject
    {
        /// <summary>
        /// Name of the type
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Identifier of the type
        /// </summary>
        string Identifier { get; set; }

        /// <summary>
        /// Severity of the type
        /// </summary>
        Severity Severity { get; set; }

        /// <summary>
        /// Indicator if notification is disabled
        /// </summary>
        bool IsDisabled { get; set; }
    }
}
