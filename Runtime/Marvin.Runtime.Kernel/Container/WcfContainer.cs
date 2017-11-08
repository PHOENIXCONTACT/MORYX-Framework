using System;
using System.Collections.Generic;
using Castle.Facilities.WcfIntegration;
using Marvin.Container;
using Marvin.Tools.Wcf;
using Component = Castle.MicroKernel.Registration.Component;

namespace Marvin.Runtime.Kernel.Container
{
    /// <summary>
    /// Container for hosting wcf services
    /// </summary>
    public class WcfContainer : LocalContainer
    {
        /// <summary>
        /// Default constructor without strategies
        /// </summary>
        public WcfContainer() : base(new Dictionary<Type, string>())
        {
            Container.AddFacility<WcfFacility>();

            Container.Register(Component.For<ITypedHostFactory>().ImplementedBy<TypedHostFactory>()
                .DynamicParameters((kernel, parameters) => parameters["kernel"] = kernel));
        }
    }
}