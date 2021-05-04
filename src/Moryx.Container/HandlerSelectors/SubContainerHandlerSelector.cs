using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Moryx.Container
{
    public class SubContainerHandlerSelector : IHandlerSelector
    {
        private readonly List<IWindsorContainer> _containers = new List<IWindsorContainer>();

        public bool HasOpinionAbout(string key, Type service)
        {
            foreach (var container in _containers)
            {
                if (container.Kernel.GetHandler(service) != null)
                {
                    return true;
                }
            }

            return false;
        }

        public IHandler SelectHandler(string key, Type service, IHandler[] handlers)
        {
            foreach (var container in _containers)
            {
                var handler = container.Kernel.GetHandler(service);
                if (handler != null)
                {
                    return handler;
                }
            }

            return null;
        }

        public void RegisterContainer(IWindsorContainer container)
        {
            if (!_containers.Contains(container))
            {
                _containers.Add(container);
            }
        }

        public void UnregisterContainer(IWindsorContainer container)
        {
            _containers.Remove(container);
        }
    }
}
