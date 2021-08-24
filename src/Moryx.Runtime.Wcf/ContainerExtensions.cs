// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Castle.Facilities.WcfIntegration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    /// <summary>
    /// Extensions to the container to register wcf behaviour
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Register wcf to the local module container
        /// </summary>
        [Obsolete("Extension with WCF factory was replaced by IEndpointHosting.ConfigureFactory !")]
        public static IContainer RegisterWcf(this IContainer container,
            IWcfHostFactory wcfHostFactory)
        {
            container.Extend<WcfFacility>();
            container.Register<ITypedHostFactory, TypedHostFactory>();
            var logger = container.Resolve<IModuleLogger>();
            var typedFactory = container.Resolve<ITypedHostFactory>();

            container.SetInstance((IConfiguredHostFactory)new ConfiguredHostFactory(wcfHostFactory)
            {
                Factory = typedFactory,
                Logger = logger
            });
            return container;
        }
    }
}