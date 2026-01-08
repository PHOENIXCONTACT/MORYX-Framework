// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container;

/// <summary>
/// Registration attribute to decorate components of a module.
/// </summary>
public class ComponentAttribute : Attribute
{
    /// <summary>
    /// Life cycle of this component
    /// </summary>
    public LifeCycle LifeStyle { get; }

    /// <summary>
    /// Implemented service
    /// </summary>
    public Type[] Services { get; }

    /// <summary>
    /// Optional name of component
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Constructor with life cycle
    /// </summary>
    /// <param name="lifeStyle">Life style of component</param>
    /// <param name="services">Implemented service</param>
    public ComponentAttribute(LifeCycle lifeStyle, params Type[] services)
    {
        LifeStyle = lifeStyle;
        Services = services;
    }
}

/// <summary>
/// Life cycle for this component
/// </summary>
public enum LifeCycle
{
    /// <summary>
    /// Create only one instance during container life time
    /// </summary>
    Singleton,

    /// <summary>
    /// Create a new instance for every request
    /// </summary>
    Transient
}