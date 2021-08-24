// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Castle.Facilities.WcfIntegration;
using Moryx.Communication.Endpoints;
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
        [Obsolete("Extension with WCF factory was replaced by hosting independent ActivateHosting extension !")]
        public static IContainer RegisterWcf(this IContainer container, IWcfHostFactory wcfHostFactory)
        {
            // TODO: Move to WcfHostFactory on removal
            container.Extend<WcfFacility>();
            container.Register<ITypedHostFactory, TypedHostFactory>();
            container.SetInstance(wcfHostFactory);
            container.Register<IConfiguredHostFactory, ConfiguredHostFactory>();
            container.Register<IEndpointHostFactory, ConfiguredHostFactory>();
            return container;
        }
    }
}