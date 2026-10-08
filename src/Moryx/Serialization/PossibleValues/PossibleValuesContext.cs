// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Serialization;

/// <summary>
/// Provide context for the <see cref="PossibleValuesAttribute"/>
/// </summary>
public class PossibleValuesContext
{
    #region Member Fields
    private Func<Type, object> _serviceProvider;
    private Func<Type, object> _localProvider;
    private Func<Type, IEnumerable<Type>> _registeredImplementation;
    #endregion

    /// <summary>
    /// Instance the current class needed for the possible values.
    /// </summary>
    public object Instance { get; }

    /// <summary>
    /// Dictionary of key/value pairs associated with this context.
    /// </summary>
    public Dictionary<object, object> Items { get; }

    /// <summary>
    /// Construct a <see cref="PossibleValuesContext"/> for a given object instance and an optional
    /// property bag of <paramref name="items"/>.
    /// </summary>
    /// <param name="instance">object being used as source for the possible value</param>
    /// <param name="items">Optional set of key/value pairs to make available to consumers via <see cref="Items"/>.
    /// If null, an empty dictionary will be created.  If not null, the set of key/value pairs will be copied into a
    /// new dictionary, preventing consumers from modifying the original dictionary.
    /// </param>
    /// <param name="provider">Global service provider for access to external facades.</param>
    /// <param name="container">Local container for the current module.</param>
    public PossibleValuesContext(object instance, IDictionary<object, object> items, IServiceProvider provider, IContainer container)
    {
        ArgumentNullException.ThrowIfNull(instance);
        Items = items != null ? new Dictionary<object, object>(items) : [];
        Instance = instance;
        if (container != null)
        {
            InitializeLocalProvider(container);
        }
        if (provider != null)
        {
            InitializeServiceProvider(provider.GetService);
        }
    }

    private void InitializeServiceProvider(Func<Type, object> serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private void InitializeLocalProvider(IContainer container)
    {
        _localProvider = container.Resolve;
        _registeredImplementation = container.GetRegisteredImplementations;
    }

    /// <summary>
    /// See <see cref="IServiceProvider.GetService(Type)" /> and <see cref="IContainer.Resolve(Type, string)" />.
    /// </summary>
    /// <param name="serviceType">The type of the service needed.</param>
    /// <returns>An instance of that service or null if it is not available.</returns>
    public object GetService(Type serviceType)
    {
        return _localProvider?.Invoke(serviceType) ?? _serviceProvider?.Invoke(serviceType);
    }

    /// <summary>
    /// Get all implementations for a given <paramref name="serviceType"/>
    /// </summary>
    /// <param name="serviceType">The type of the service needed.</param>
    /// <returns>Implementations of <paramref name="serviceType"/>.</returns>
    public IEnumerable<Type> GetRegisteredImplementations(Type serviceType)
    {
        return _registeredImplementation?.Invoke(serviceType);
    }
}
