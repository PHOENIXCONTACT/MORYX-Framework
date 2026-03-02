// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Launcher;

/// <summary>
/// Extensions methods for the <see cref="ILauncher"/>
/// </summary>
public static class LauncherExtensions
{
    extension(ILauncher launcher)
    {
        /// <summary>
        /// Returns the list of all launcher regions
        /// </summary>
        /// <param name="launcher"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public RegionItem GetRegion(Func<RegionItem, bool> filter)
        {
            return launcher.GetRegions().FirstOrDefault(filter);
        }
    }
}