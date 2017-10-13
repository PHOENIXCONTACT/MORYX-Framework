using System;
using System.Collections.Generic;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Marvin.Tools.Wcf;

namespace Marvin.Container
{
    /// <summary>
    /// Internal module container for server module
    /// </summary>
    public class ServerLocalContainer : LocalContainer
    {
        /// <summary>
        /// Default constructor without strategies
        /// </summary>
        public ServerLocalContainer() : this(new Dictionary<Type, string>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerLocalContainer"/> class.
        /// </summary>
        /// <param name="config">Configuration for the container.</param>
        public ServerLocalContainer(IDictionary<Type, string> config)
            : base(config)
        {
            Container.AddFacility<WcfFacility>();

            Container.Register(Component.For<ITypedHostFactory>().ImplementedBy<TypedHostFactory>()
                                        .DynamicParameters((kernel, parameters) => parameters["kernel"] = kernel));
        }
    }
}