// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.ProcessData.Adapter.ResourceManagement;

/// <summary>
/// Module configuration of the adapter <see cref="ModuleController"/>
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
}