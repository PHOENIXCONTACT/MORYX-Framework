// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Serialization;

namespace Moryx.Maintenance.Management.ModuleController;

/// <summary>
/// Module configuration of the <see cref="ModuleController"/>
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
    [Display(Description = "All maintenance orders will be refreshed/updated periodically. Value in Milliseconds")]
    [EntrySerialize, DefaultValue(60 * 60 * 1000), DataMember] //1h default
    public int RefreshPeriodMs { get; set; }
}