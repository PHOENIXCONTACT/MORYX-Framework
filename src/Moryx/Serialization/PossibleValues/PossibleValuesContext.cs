// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Serialization;

/// <summary>
/// Provide context for the <see cref="PossibleValuesAttribute"/>
/// </summary>
/// <param name="instance">object being used as source for the possible value
/// If null, an empty dictionary will be created.  If not null, the set of key/value pairs will be copied into a
/// new dictionary, preventing consumers from modifying the original dictionary.
/// </param>
/// <param name="provider">Global service provider for access to external facades.</param>
/// <param name="container">Local container for the current module.</param>
public class PossibleValuesContext(object instance, IServiceProvider provider, IContainer container)
{
    #region Member Fields
    private readonly IServiceProvider _serviceProvider = provider;
    private readonly IContainer _localProvider = container;
    #endregion

    /// <summary>
    /// Instance of the current class needed for the possible values.
    /// </summary>
    public object Instance { get; } = instance;
   
    /// <summary>
    /// See <see cref="IServiceProvider.GetService(Type)" /> and <see cref="IContainer.Resolve(Type, string)" />.
    /// </summary>
    /// <param name="serviceType">The type of the service needed.</param>
    /// <returns>An instance of that service or null if it is not available.</returns>
    public object GetService(Type serviceType)
    {
        return _localProvider.Resolve(serviceType) ?? _serviceProvider.GetService(serviceType);
    }

    /// <summary>
    /// Get all implementations for a given <paramref name="serviceType"/>
    /// </summary>
    /// <param name="serviceType">The type of the service needed.</param>
    /// <returns>Implementations of <paramref name="serviceType"/>.</returns>
    public IEnumerable<Type> GetRegisteredImplementations(Type serviceType)
    {
        return _localProvider.GetRegisteredImplementations(serviceType);
    }
}
