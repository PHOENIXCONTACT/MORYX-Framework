// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Orders.Management;

/// <summary>
/// Configuration for the <see cref="OrdersLifecycleHook"/> defining which operations to create on startup
/// </summary>
[ProvidedConfig("LifecycleHooks:Orders")]
public sealed class OrdersLifecycleHookConfig : ConfigBase
{
    /// <summary>
    /// List of operations to create
    /// </summary>
    [DataMember]
    public OperationImportConfig[] Operations { get; set; }
}
