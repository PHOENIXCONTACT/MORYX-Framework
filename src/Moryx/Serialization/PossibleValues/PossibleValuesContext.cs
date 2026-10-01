// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Serialization;

/// <summary>
/// Provide context for the <see cref="PossibleValuesAttribute"/>
/// </summary>
public class PossibleValuesContext
{
    /// <summary>
    /// Instance the current class needed for the possible values.
    /// </summary>
    public object Instance { get; }

    /// <summary>
    /// Dictionary of key/value pairs associated with this context.
    /// </summary>
    public Dictionary<object, object> Items { get; }

    /// <summary>
    /// Global container
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Local container
    /// </summary>
    public IContainer Container { get; }

    /// <summary>
    /// Construct a <see cref="PossibleValuesContext"/> for a given object instance and an optional
    /// property bag of <paramref name="items"/>.
    /// </summary>
    /// <param name="instance">object being used as source for the possible value</param>
    /// <param name="items">Optional set of key/value pairs to make available to consumers via <see cref="Items"/>.
    /// If null, an empty dictionary will be created.  If not null, the set of key/value pairs will be copied into a
    /// new dictionary, preventing consumers from modifying the original dictionary.
    /// </param>
    /// <param name="provider">Global container. </param>
    /// <param name="container">Local container. </param>
    public PossibleValuesContext(object instance, IDictionary<object, object> items, IServiceProvider provider, IContainer container)
    {
        ArgumentNullException.ThrowIfNull(instance);
        Items = items != null ? new Dictionary<object, object>(items) : [];
        Instance = instance;
        ServiceProvider = provider;
        Container = container;
    }
}
