// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Orders.Management;

/// <summary>
/// Configuration of operation notifications
/// </summary>
[DataContract]
public class OperationNotificationConfig
{
    /// <summary>
    /// Enable Amount-Reached Notification
    /// </summary>
    [DataMember]
    [DisplayName("Enable Amount-Reached Notification")]
    public bool EnableAmountReachedNotification { get; set; }
}
