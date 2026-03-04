// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0


/// <summary>
/// Decorator for partial view (Region)
/// </summary>
/// <remarks>
/// Export view as region under given name
/// </remarks>
public class LauncherRegionAttribute(string name) : Attribute
{
    /// <summary>
    /// Unique name of the region
    /// </summary>
    public string Name { get; } = name;

}
