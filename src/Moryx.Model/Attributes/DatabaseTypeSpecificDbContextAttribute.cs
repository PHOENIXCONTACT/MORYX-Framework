// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model.Attributes;

/// <summary>
/// Attribute to identify database specific contexts
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public abstract class DatabaseTypeSpecificDbContextAttribute : Attribute
{
    /// <summary>
    /// Type of the belonging model configurator type
    /// </summary>
    public Type ModelConfiguratorType { get; }

    /// <summary>
    /// Type of the base DbContext-type
    /// </summary>
    public Type BaseDbContextType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseTypeSpecificDbContextAttribute"/> class.
    /// </summary>
    /// <param name="modelConfiguratorType">Type of the belonging model configurator type</param>
    /// <param name="baseDbContextType">Type of the base DbContext-type</param>
    protected DatabaseTypeSpecificDbContextAttribute(Type modelConfiguratorType, Type baseDbContextType)
    {
        ModelConfiguratorType = modelConfiguratorType;
        BaseDbContextType = baseDbContextType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseTypeSpecificDbContextAttribute"/> class.
    /// </summary>
    /// <param name="modelConfiguratorType">Type of the belonging model configurator type</param>
    protected DatabaseTypeSpecificDbContextAttribute(Type modelConfiguratorType)
    {
        ModelConfiguratorType = modelConfiguratorType;
    }
}
