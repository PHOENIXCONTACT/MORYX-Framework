// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Core;
using Castle.MicroKernel.Context;
using Castle.MicroKernel;

namespace Moryx.Container
{
    internal class StrategySubResolver : ISubDependencyResolver
    {
        private readonly IKernel _kernel;
        private readonly IDictionary<Type, string> _strategies;

        public StrategySubResolver(IKernel kernel, IDictionary<Type, string> strategies)
        {
            _kernel = kernel;
            _strategies = strategies;
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType != null && _strategies.ContainsKey(dependency.TargetType);
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            // The dependency is a registered strategy, resolve by strategy name instead
            return _kernel.Resolve(_strategies[dependency.TargetType], dependency.TargetType);
        }
    }
}
