// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Moryx.Operators.Management;

/// <summary>
/// Module configuration of the <see cref="ModuleController"/>
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
    /// <summary>
    /// Configurable card number to identify the default operator
    /// </summary>
    [DataMember, Description("Cardnumber of the default operator")]
    [DefaultValue("9999")]
    public string DefaultOperator { get; set; }
}
