// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Launcher;

/// <summary>
/// Configuration for the launcher
/// </summary>
[DataContract]
public class LauncherConfig : ConfigBase
{
    /// <summary>
    /// Sort indices for modules
    /// </summary>
    [DataMember]
    public ModuleSortIndexConfig[] ModuleSortIndices { get; set; }

    /// <summary>
    /// Definition of external modules
    /// </summary>
    [DataMember]
    public ExternalModuleConfig[] ExternalModules { get; set; }
}
