using System;
using System.Collections.Generic;
using Castle.Windsor;

namespace Moryx.Container
{
    public class LocalContainerWithSubContainer : LocalContainer
    {
        private readonly SubContainerHandlerSelector _handlerSelector = new SubContainerHandlerSelector();

        /// <summary>
        /// Default constructor without strategies
        /// </summary>
        public LocalContainerWithSubContainer() : this(new Dictionary<Type, string>())
        {
        }

        /// <summary>
        /// Constructor for the local castle container.
        /// </summary>
        /// <param name="strategies">Configuration for the container.</param>
        public LocalContainerWithSubContainer(IDictionary<Type, string> strategies)
            : base(strategies)
        {
        }

        public IWindsorContainer InternalContainer => Container;

        public void Initialize()
        {
            Container.Kernel.AddHandlerSelector(_handlerSelector);
        }

        public void RegisterContainer(IContainer container)
        {
            if (container is CastleContainer castleContainer)
            {
                _handlerSelector.RegisterContainer(castleContainer.Container);
            }
        }

        public void UnregisterContainer(IContainer container)
        {
            if (container is CastleContainer castleContainer)
            {
                _handlerSelector.UnregisterContainer(castleContainer.Container);
            }
        }
    }
}
