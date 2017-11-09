using System;
using System.Collections.Generic;
using Castle.Facilities.WcfIntegration;
using Marvin.Container;
using Marvin.Tools.Wcf;
using Component = Castle.MicroKernel.Registration.Component;

namespace Marvin.Runtime.Container
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