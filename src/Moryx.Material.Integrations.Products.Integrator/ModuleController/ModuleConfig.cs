// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Material.Linking;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Products.Integrator;

/// <summary>
/// Module configuration listing the <see cref="Linking.ILinkingHook"/> plugins that should
/// be executed by the <c>LinkingHookManager</c>.
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
    /// <summary>
    /// Ordered list of hook plugins to be executed when a product is linked to a material container.
    /// </summary>
    [DataMember]
    [Display(Name = "Linking Hooks", Description = "Ordered list of hook plugins to be executed when a product is linked to a material container.")]
    [PluginConfigs(typeof(ILinkingHook), exportBaseType: false)]
    public List<ProductLinkingHookConfig> Hooks { get; set; } = [];
}