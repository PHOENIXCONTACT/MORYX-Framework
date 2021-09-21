using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Moryx.Container;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Sub resolver to fetch dependencies for controllers from a MORYX container
    /// </summary>
    internal class ControllerProxySubResolver : ISubDependencyResolver
    {
        public ControllerProxySubResolver(Type controller, IContainer container)
        {
            Controller = controller;
            Container = container;
        }
        public Type Controller { get; }
        public IContainer Container { get; }

        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            // Each proxy resolver only serves its target controller
            return model.Implementation == Controller && Container.GetRegisteredImplementations(dependency.TargetType).Any();
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return Container.Resolve(dependency.TargetType);
        }
    }
}