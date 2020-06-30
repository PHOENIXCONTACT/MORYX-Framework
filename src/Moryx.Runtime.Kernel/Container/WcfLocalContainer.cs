// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Castle.Facilities.WcfIntegration;
using Marvin.Container;
using Marvin.Tools.Wcf;
using Component = Castle.MicroKernel.Registration.Component;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Container for hosting wcf services
    /// </summary>
    public class WcfLocalContainer : LocalContainer
    {
        /// <summary>
        /// Default constructor without strategies
        /// </summary>
        public WcfLocalContainer() : this(new Dictionary<Type, string>())
        {
            
        }

        /// <summary>
        /// Constructor with strategies
        /// </summary>
        public WcfLocalContainer(IDictionary<Type, string> strategies) : base(strategies)
        {
            Container.AddFacility<WcfFacility>();

            Container.Register(Component.For<ITypedHostFactory>().ImplementedBy<TypedHostFactory>()
                .DynamicParameters((kernel, parameters) => parameters["kernel"] = kernel));
        }
    }
}
