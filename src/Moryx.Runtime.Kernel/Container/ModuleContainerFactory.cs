// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel;

/// <summary>
/// Factory to create local containers of <see cref="IServerModule"/>
/// </summary>
public class ModuleContainerFactory : IModuleContainerFactory
{
    private readonly IServiceProvider _serviceProvider;

    // TODO: Change in next major - remove parameterless constructor and make IServiceProvider non-optional
    /// <summary>
    /// Parameterless constructor for backward compatibility
    /// </summary>
    public ModuleContainerFactory()
    {
    }

    /// <summary>
    /// Creates a new instance with an <see cref="IServiceProvider"/> to bridge the ServiceProvider
    /// into module containers.
    /// </summary>
    public ModuleContainerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public IContainer Create(IDictionary<Type, string> strategies, Assembly moduleAssembly)
    {
        var container = new CastleContainer(strategies, _serviceProvider, BridgeTypeFilter);
        container.LoadFromAssembly(moduleAssembly);
        return container;
    }

    /// <summary>
    /// Filter that prevents module infrastructure types from being resolved through the service provider bridge.
    /// Facades must be resolved through the facade mechanism, not directly from the ServiceProvider.
    /// <see cref="IModuleManager"/> must not be accessible from within modules.
    /// </summary>
    private static bool BridgeTypeFilter(Type type) =>
        !typeof(IFacadeControl).IsAssignableFrom(type)
        && !typeof(IModuleManager).IsAssignableFrom(type);
}
