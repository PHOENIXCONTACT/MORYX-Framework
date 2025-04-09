// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Core;
using Castle.MicroKernel.Context;
using Castle.MicroKernel;
using System.Reflection;

namespace Moryx.Container
{
    internal class NamedDependencyResolver : ISubDependencyResolver
    {
        private readonly IKernel _kernel;

        public NamedDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType != null && GetAttribute(model.Implementation, dependency) != null;
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            // The dependency is declared with a name
            var att = GetAttribute(model.Implementation, dependency);
            // The dependency is a registered strategy, resolve by strategy name instead
            return _kernel.Resolve(att.ComponentName, dependency.TargetType);
        }

        private static NamedAttribute GetAttribute(Type targetType, DependencyModel dependency)
        {
            NamedAttribute att = null;
            if (dependency.IsOptional)
            {
                // Find property and look for attribute
                var property = targetType.GetProperty(dependency.DependencyKey);
                att = property.GetCustomAttribute<NamedAttribute>();
            }
            else if (dependency is ConstructorDependencyModel constDep)
            {
                var parameter = constDep.Constructor.Constructor.GetParameters().FirstOrDefault(param => param.Name == dependency.DependencyKey);
                att = parameter.GetCustomAttribute<NamedAttribute>();
            }
            return att;
        }
    }
}
