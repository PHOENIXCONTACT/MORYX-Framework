// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Material.Integrations.Orders.Integrator;

/// <summary>
/// Module configuration listing the <see cref="Linking.LinkingHook"/> plugins that should
/// be executed by the <c>LinkingHookManager</c>.
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
    /// <summary>
    /// Ordered list of hook plugin names. Each entry references a registered
    /// <see cref="Linking.LinkingHook"/> plugin by component name.
    /// </summary>
    [DataMember, Description("Ordered list of LinkingHook plugin names to execute.")]
    public List<string> Hooks { get; set; } = new();
}