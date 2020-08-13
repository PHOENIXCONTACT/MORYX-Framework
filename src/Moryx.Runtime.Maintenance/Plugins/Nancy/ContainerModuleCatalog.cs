using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.Container;
using Nancy;

namespace Moryx.Runtime.Maintenance.Plugins
{
    internal class ContainerModuleCatalog : INancyModuleCatalog
    {
        private readonly IContainer _container;

        public ContainerModuleCatalog(IContainer container)
        {
            _container = container;
        }

        public IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            var modules = _container.ResolveAll<INancyModule>().ToArray();
            return modules;
        }

        public INancyModule GetModule(Type moduleType, NancyContext context)
        {
            return (INancyModule)_container.Resolve(moduleType);
        }
    }
}