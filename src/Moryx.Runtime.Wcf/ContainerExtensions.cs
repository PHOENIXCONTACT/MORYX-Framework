// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Facilities.WcfIntegration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    public static class ContainerExtensions
    {
        /// <summary>
        /// Register wcf to the local module container
        /// </summary>
        public static void RegisterWcf(this IContainer container,
            IWcfHostFactory wcfHostFactory, IModuleLogger logger)
        {
            container.Extend<WcfFacility>();
            container.Register<ITypedHostFactory, TypedHostFactory>();
            var typedFactory = container.Resolve<ITypedHostFactory>();

            container.SetInstance((IConfiguredHostFactory)new ConfiguredHostFactory(wcfHostFactory)
            {
                Factory = typedFactory,
                Logger = logger
            });
        }
    }
}