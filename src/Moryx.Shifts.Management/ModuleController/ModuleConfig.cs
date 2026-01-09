// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Shifts.Management;

/// <summary>
/// Module configuration of the <see cref="ModuleController"/>
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
}