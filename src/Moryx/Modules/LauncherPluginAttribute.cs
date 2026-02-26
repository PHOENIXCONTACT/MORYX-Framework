// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Modules;

/// <summary>
/// Decorator for partial view (Region)
/// </summary>
/// <remarks>
/// Export view as plugin under given name
/// </remarks>
public class LauncherPluginAttribute(string name) : Attribute
{
    /// <summary>
    /// Unique name of the plugin
    /// </summary>
    public string Name { get; } = name;

}
