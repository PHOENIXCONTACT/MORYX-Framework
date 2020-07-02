// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Notifications
{
    /// <summary>
    /// Certificate for a published notification with source identity and
    /// removal validation
    /// </summary>
    public interface INotificationCertificate
    {
        /// <summary>
        /// Source and owner of the certificate, publisher of the notification
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Validate a removal signature for the notification
        /// </summary>
        bool Validate(INotification notification, string signature);
    }
}