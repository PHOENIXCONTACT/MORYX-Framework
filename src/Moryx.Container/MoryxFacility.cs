// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace Moryx.Container
{
    internal class MoryxFacility : IFacility
    {
        private IDictionary<Type, string> _strategies;

        public void AddStrategies(IDictionary<Type, string> strategies)
        {
            _strategies = strategies;
        }

        public void Init(IKernel kernel, IConfiguration facilityConfig)
        {
            kernel.Register(Component.For<INameBasedComponentSelector>().ImplementedBy<NameBasedComponentSelector>().LifestyleTransient());
            kernel.Register(Component.For<IConfigBasedComponentSelector>().ImplementedBy<ConfigBasedComponentSelector>().LifestyleTransient());
            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel, true));
            kernel.Resolver.AddSubResolver(new NamedDependencyResolver(kernel));
            kernel.Resolver.AddSubResolver(new StrategySubResolver(kernel, _strategies));
            kernel.Resolver.AddSubResolver(new ChildContainerSubResolver(kernel));
        }

        public void Terminate()
        {
        }
    }
}
