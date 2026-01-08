// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Modules;

/// <summary>
/// Category of web modules to structure the shell navigation
/// </summary>
public enum ModuleCategory
{
    /// <summary>
    /// Module is displayed in the navigation section for users
    /// </summary>
    User = 0,

    /// <summary>
    /// Module contains settings and displayed in a sub-menu
    /// </summary>
    Settings = 10,

    /// <summary>
    /// Module offers insights into the application for diagnostics and trouble shooting
    /// </summary>
    Diagnostics = 20,

    /// <summary>
    /// Module offers support and help
    /// </summary>
    Help = 30,
}