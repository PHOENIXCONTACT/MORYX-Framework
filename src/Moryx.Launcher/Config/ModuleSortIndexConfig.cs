// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Launcher;

/// <summary>
/// Configuration for module sort indices
/// </summary>
[DataContract]
public class ModuleSortIndexConfig
{
    /// <summary>
    /// Base route of the module
    /// </summary>
    [DataMember]
    public string Route { get; set; }

    /// <summary>
    /// Target index of the module in the shell navigation
    /// </summary>
    [DataMember]
    public int SortIndex { get; set; }
}
