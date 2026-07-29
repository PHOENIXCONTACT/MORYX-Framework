// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Orders.Integrator;

public class OrderLinkingHookConfig : IPluginConfig
{
    [DataMember, Description("Name of the order linking hook")]
    [PluginNameSelector(typeof(OrderLinkingHook))]
    public virtual string PluginName { get; set; }
}
