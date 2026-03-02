// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Launcher;

/// <summary>
/// Describes a region in the launcher
/// </summary>
internal class RegionItem
{
    /// <summary>
    /// Name of the partial for the given region
    /// </summary>
    public string PartialView { get; set; }

    /// <summary>
    /// The name of the region for the launcher
    /// </summary>
    public LauncherRegion Region { get; set; }
}
