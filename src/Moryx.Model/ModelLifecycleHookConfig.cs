// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Model;

[ProvidedConfig("LifecycleHooks:Model")]
public sealed class ModelLifecycleHookConfig : ConfigBase
{
    /// <summary>
    /// Allows disabling this config entry
    /// </summary>
    [DataMember]
    public bool Disabled { get; set; }

    /// <summary>
    /// Delete all dbs on startup
    /// </summary>
    [DataMember]
    public bool DeleteAllDbs { get; set; }

    /// <summary>
    /// Allows deleting only specific databases by the context name
    /// </summary>
    [DataMember]
    public string[] DbsToDelete { get; set; }

    /// <summary>
    /// Create all missing dbs
    /// </summary>
    [DataMember]
    public bool CreateDbs { get; set; }

    /// <summary>
    /// Configures when to run this hook
    /// </summary>
    [DataMember]
    public int Priority { get; set; }
}
