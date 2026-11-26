// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Products.Integrator;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
/// <summary>
/// Plugin configuration base class for a configured <see cref="ProductLinkingHook"/> instance.
/// </summary>
public class ProductLinkingHookConfig : IPluginConfig
{
    /// <inheritdoc />
    [DataMember, Description("Name of the product linking hook")]
    [PluginNameSelector(typeof(ProductLinkingHook))]
    public virtual string PluginName { get; set; }

}