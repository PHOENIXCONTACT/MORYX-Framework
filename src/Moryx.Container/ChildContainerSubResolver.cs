// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;

namespace Moryx.Container
{
    internal class ChildContainerSubResolver : ISubDependencyResolver
    {
        private readonly IKernel _kernel;

        public ChildContainerSubResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType != null && GetAttribute(model.Implementation, dependency) != null;
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            // Get attribute
            var att = GetAttribute(model.Implementation, dependency);

            // Determine the parents type and instance from the container
            var targetType = dependency.TargetType;
            var containerChild = targetType.GetInterfaces().FirstOrDefault(IsContainerChild);
            var parentType = containerChild == null ? targetType : containerChild.GetGenericArguments()[0];
            var parent = att.ParentName == null
                ? (INamedChildContainer<object>)_kernel.Resolve(parentType)
                : (INamedChildContainer<object>)_kernel.Resolve(att.ParentName, parentType);

            // Resolve child
            return parent.GetChild(att.ChildName, model.Implementation);
        }

        private static bool IsContainerChild(Type candidate)
        {
            return candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IContainerChild<>);
        }

        private static UseChildAttribute GetAttribute(Type targetType, DependencyModel dependency)
        {
            UseChildAttribute att = null;
            if (dependency.IsOptional)
            {
                // Find property and look for attribute
                var property = targetType.GetProperty(dependency.DependencyKey);
                att = property.GetCustomAttribute<UseChildAttribute>();
            }
            else if (dependency is ConstructorDependencyModel constDep)
            {
                var parameter = constDep.Constructor.Constructor.GetParameters().FirstOrDefault(param => param.Name == dependency.DependencyKey);
                att = parameter.GetCustomAttribute<UseChildAttribute>();
            }
            return att;
        }
    }
}
