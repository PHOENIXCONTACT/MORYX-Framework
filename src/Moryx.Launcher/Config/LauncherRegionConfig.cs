// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Launcher;

/// <summary>
/// Configuration for a region in the UI
/// </summary>
[DataContract]
public class LauncherRegionConfig
{
    /// <summary>
    /// Region of the launcher
    /// </summary>
    [DataMember]
    public LauncherRegion Region { get; set; }

    [DataMember]
    public string PluginName { get; set; }
}

public enum LauncherRegion
{
    Right,
}