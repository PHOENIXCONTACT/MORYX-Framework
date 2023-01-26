// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace Moryx.Container
{
    internal class MoryxFacility : IFacility
    {
        public void Init(IKernel kernel, IConfiguration facilityConfig)
        {
            kernel.Register(Component.For<INameBasedComponentSelector>().ImplementedBy<NameBasedComponentSelector>().LifestyleTransient());
            kernel.Register(Component.For<IConfigBasedComponentSelector>().ImplementedBy<ConfigBasedComponentSelector>().LifestyleTransient());
            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel, true));
            kernel.Resolver.AddSubResolver(new ChildContainerSubResolver(kernel));
        }

        public void Terminate()
        {
        }
    }
}
