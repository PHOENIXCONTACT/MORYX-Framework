// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Serialization;

namespace Moryx.Material.Management;

/// <summary>
/// Module configuration of the <see cref="ModuleController"/>.
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
    // Reserved for future configurable behavior (e.g., lineage retention, fulfillment policy).
    /// <summary>
    /// Default serialization setting for container types found in AppDomain,
    /// Hide type not configured in 'StepSettings'.
    /// </summary>
    [DataMember]
    [Description("Hide type not configured in 'StepSettings'")]
    public bool HideUnknown { get; set; }

    /// <summary>
    /// Settings for steps
    /// </summary>
    [DataMember]
    public List<ContainerTypeConfig> ContainerTypeSettings { get; set; } = [];
}

/// <summary>
/// Config for a container type
/// </summary>
[DataContract]
public class ContainerTypeConfig
{
    /// <summary>
    /// Type of step represented by this entry, Uses type Fullname.
    /// </summary>
    [DataMember]
    [PossibleTypes(typeof(IMaterialContainer), UseFullname = true)]
    public string Type { get; set; }

    /// <summary>
    /// Visibility setting
    /// </summary>
    [DataMember]
    public bool Enabled { get; set; }
}
