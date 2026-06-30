// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Serialization;

namespace Moryx.Runtime.Hooks;

[ProvidedConfig("Hooks:Databases")]
public sealed class DatabaseHookConfig : ConfigBase
{
    /// <summary>
    /// Allows disabling this config entry
    /// </summary>
    [DataMember, EntrySerialize]
    public bool Disabled { get; set; }

    /// <summary>
    /// Delete all dbs on startup
    /// </summary>
    [DataMember, EntrySerialize]
    public bool DeleteAllDbs { get; set; }

    /// <summary>
    /// Allows deleting only specific databases by the context name
    /// </summary>
    [DataMember, EntrySerialize]
    public string[]? DbsToDelete { get; set; }

    /// <summary>
    /// Create all missing dbs
    /// </summary>
    [DataMember, EntrySerialize]
    public bool CreateDbs { get; set; }

    /// <summary>
    /// Configures when to run this hook
    /// </summary>
    [DataMember, EntrySerialize]
    public int Priority { get; internal set; }
}
