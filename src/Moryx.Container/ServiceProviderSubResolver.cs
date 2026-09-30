// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;

namespace Moryx.Container;

/// <summary>
/// Castle Windsor sub-resolver that delegates to an <see cref="IServiceProvider"/>
/// for types not registered in Windsor. This bridges the ServiceProvider into module containers.
/// </summary>
internal class ServiceProviderSubResolver(IKernel kernel, IServiceProvider serviceProvider, Predicate<Type> typeFilter = null) : ISubDependencyResolver
{
    public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dep)
    {
        // Only bridge to the ServiceProvider if the type is not already registered in Windsor
        // and the optional type filter does not exclude it
        return dep.TargetType != null
            && !kernel.HasComponent(dep.TargetType)
            && (typeFilter == null || typeFilter(dep.TargetType))
            && serviceProvider.GetService(dep.TargetType) != null;
    }

    public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dep)
    {
        var service = serviceProvider.GetService(dep.TargetType);
        if (service == null)
        {
            throw new InvalidOperationException($"Service of type '{dep.TargetType}' could not be resolved from IServiceProvider.");
        }

        return service;
    }
}
