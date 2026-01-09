// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Launcher;

/// <summary>
/// Configuration for an external module
/// </summary>
public class ExternalModuleConfig
{
    /// <summary>
    /// Title of the module
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Url which will be embedded
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Icon of the module from material icons
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Description of the module
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Internal route to access the module
    /// </summary>
    public string Route { get; set; }
}
