// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;

namespace Moryx.Launcher
{
    /// <summary>
    /// Component to determine navigation items for the shell
    /// </summary>
    public interface IShellNavigator
    {
        /// <summary>
        /// Gathers the available Web Modules to display in the shell
        /// </summary>
        Task<IReadOnlyList<ModuleItem>> GetModuleItems(HttpContext context);
    }
}
