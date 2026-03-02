
// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Launcher;

/// <summary>
/// Component to determine items for the shell
/// </summary>
internal interface ILauncher : IShellNavigator
{
    /// <summary>
    /// Get the regions configuration
    /// </summary>
    RegionItem GetRegion(LauncherRegion region);
}
