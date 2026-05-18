
// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;

namespace Moryx.Launcher;

/// <summary>
/// Component to determine module/regin items for the shell
/// </summary>
internal interface INavigation
{
    /// <summary>
    /// Gathers the available Web Modules to display in the shell
    /// </summary>
    Task<IReadOnlyList<ModuleItem>> GetModuleItemsAsync(HttpContext context);

    /// <summary>
    /// Get the regions configuration
    /// </summary>
    RegionItem GetRegion(LauncherRegion region);
}
