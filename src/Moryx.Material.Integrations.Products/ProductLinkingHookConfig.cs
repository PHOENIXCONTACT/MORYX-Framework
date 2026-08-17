// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Products.Integrator;

/// <summary>
/// Plugin configuration for a configured <see cref="ProductLinkingHook"/> instance.
/// </summary>
public class ProductLinkingHookConfig : IPluginConfig
{
    /// <inheritdoc />
    [DataMember, Description("Name of the product linking hook")]
    [PluginNameSelector(typeof(ProductLinkingHook))]
    public virtual string PluginName { get; set; } = string.Empty;
}