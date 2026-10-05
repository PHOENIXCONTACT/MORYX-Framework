// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace Moryx.Container;

internal class MoryxFacility : IFacility
{
    private IDictionary<Type, string> _strategies;
    private IServiceProvider _serviceProvider;
    private Predicate<Type> _serviceProviderTypeFilter;

    public void AddStrategies(IDictionary<Type, string> strategies)
    {
        _strategies = strategies;
    }

    /// <summary>
    /// Set the <see cref="IServiceProvider"/> to bridge the ServiceProvider into the container.
    /// </summary>
    /// <param name="serviceProvider">The ASP.NET Core service provider</param>
    /// <param name="typeFilter">Optional filter to exclude types from bridging (e.g. facades)</param>
    public void SetServiceProvider(IServiceProvider serviceProvider, Predicate<Type> typeFilter = null)
    {
        _serviceProvider = serviceProvider;
        _serviceProviderTypeFilter = typeFilter;
    }

    public void Init(IKernel kernel, IConfiguration facilityConfig)
    {
        kernel.Register(Component.For<INameBasedComponentSelector>().ImplementedBy<NameBasedComponentSelector>().LifestyleTransient());
        kernel.Register(Component.For<IConfigBasedComponentSelector>().ImplementedBy<ConfigBasedComponentSelector>().LifestyleTransient());
        kernel.Resolver.AddSubResolver(new CollectionResolver(kernel, true));
        kernel.Resolver.AddSubResolver(new NamedDependencyResolver(kernel));
        kernel.Resolver.AddSubResolver(new StrategySubResolver(kernel, _strategies));
        kernel.Resolver.AddSubResolver(new ChildContainerSubResolver(kernel));

        // Bridge to ServiceProvider - added last so Windsor-registered components take precedence
        if (_serviceProvider != null)
            kernel.Resolver.AddSubResolver(new ServiceProviderSubResolver((IKernelInternal)kernel, _serviceProvider, _serviceProviderTypeFilter));
    }

    public void Terminate()
    {
    }
}