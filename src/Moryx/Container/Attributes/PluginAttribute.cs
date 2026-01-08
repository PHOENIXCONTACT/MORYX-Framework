// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container;

/// <summary>
/// Registration attribute for local container plugins
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PluginAttribute : ComponentAttribute
{
    /// <summary>
    /// Constructor with life cycle
    /// </summary>
    /// <param name="lifeStyle">Life style of component</param>
    /// <param name="services">Implemented service</param>
    public PluginAttribute(LifeCycle lifeStyle, params Type[] services)
        : base(lifeStyle, services)
    {
    }
}