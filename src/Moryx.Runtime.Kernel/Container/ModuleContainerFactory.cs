// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel;

/// <summary>
/// Factory to create local containers of <see cref="IServerModule"/>
/// </summary>
public class ModuleContainerFactory(IHttpClientFactory httpClientFactory = null, TimeProvider timeProvider = null) : IModuleContainerFactory
{
    /// <summary>
    /// Parameterless constructor for compatibility
    /// </summary>
    public ModuleContainerFactory() : this(null, null)
    {
    }

    /// <inheritdoc />
    public IContainer Create(IDictionary<Type, string> strategies, Assembly moduleAssembly)
    {
        var container = new CastleContainer(strategies);
        container.LoadFromAssembly(moduleAssembly);

        if (httpClientFactory is not null)
        {
            container.SetInstance(httpClientFactory);
        }
        container.SetInstance(timeProvider ?? TimeProvider.System);

        return container;
    }
}
