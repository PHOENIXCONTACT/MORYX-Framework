// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Orders.Integrator;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
/// <summary>
/// Plugin configuration base class for a configured <see cref="OrderLinkingHook"/> instance.
/// </summary>
public class OrderLinkingHookConfig : IPluginConfig
{
    /// <inheritdoc/>
    [DataMember, Description("Name of the order linking hook")]
    [PluginNameSelector(typeof(OrderLinkingHook))]
    public virtual string PluginName { get; set; }
}
