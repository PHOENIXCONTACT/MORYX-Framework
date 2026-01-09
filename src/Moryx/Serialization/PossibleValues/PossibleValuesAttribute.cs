// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Serialization;

/// <summary>
/// Base attribute for all attributes that support multiple values
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public abstract class PossibleValuesAttribute : Attribute
{
    /// <summary>
    /// Flag if this member implements its own string to value conversion
    /// </summary>
    public abstract bool OverridesConversion { get; }

    /// <summary>
    /// Flag if new values shall be updated from the old value
    /// </summary>
    public abstract bool UpdateFromPredecessor { get; }

    /// <summary>
    /// Extract possible values from local or global DI registration
    /// </summary>
    public abstract IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider);

    /// <summary>
    /// Parse value from string using local or global DI container
    /// </summary>
    /// <param name="container">Module local DI container</param>
    /// <param name="serviceProvider">Global service registration</param>
    /// <param name="value">Value to parse</param>
    public virtual object Parse(IContainer container, IServiceProvider serviceProvider, string value)
    {
        return value;
    }
}