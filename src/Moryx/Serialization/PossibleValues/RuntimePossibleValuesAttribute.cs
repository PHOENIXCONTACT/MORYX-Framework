// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Serialization.PossibleValues;

/// <summary>
/// Base attribute for all attributes that support multiple values ast runtime
/// </summary>
public abstract class RuntimePossibleValuesAttribute : PossibleValuesAttribute
{
    /// <summary>
    /// Name of the property to use as values source in the current class
    /// </summary>
    public string Source { get; set; }

    /// <inheritdoc/>
    public override IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)
    {
        return GetValues(null, localContainer, serviceProvider);
    }

    /// <inheritdoc/>
    public override object Parse(IContainer container, IServiceProvider serviceProvider, string value)
    {
        return Parse(null, container, serviceProvider, value);
    }

    /// <summary>
    /// Parse value from string using local or global DI container
    /// </summary>
    /// <param name="instance">instance of the class in which the <see cref="Source"/> exist</param>
    /// <param name="container">Module local DI container</param>
    /// <param name="serviceProvider">Global service registration</param>
    /// <param name="value">Value to parse</param>
    public abstract object Parse(object instance, IContainer container, IServiceProvider serviceProvider, string value);

    /// <summary>
    /// Extract possible values from the <paramref name="instance"/> or local or global DI registration
    /// </summary>
    /// <param name="instance">instance of the class in which the <see cref="Source"/> exist</param>
    /// <param name="localContainer">Module local DI container</param>
    /// <param name="serviceProvider">Global service registration</param>

    public abstract IEnumerable<string> GetValues(object instance, IContainer localContainer, IServiceProvider serviceProvider);
}
