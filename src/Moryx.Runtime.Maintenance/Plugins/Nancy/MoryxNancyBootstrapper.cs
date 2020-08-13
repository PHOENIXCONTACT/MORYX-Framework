using Moryx.Container;
using Nancy;
using Nancy.TinyIoc;

namespace Moryx.Runtime.Maintenance.Plugins
{
    internal class MoryxNancyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IContainer _container;

        public MoryxNancyBootstrapper(IContainer container)
        {
            _container = container;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<INancyModuleCatalog>(new ContainerModuleCatalog(_container));
            container.Register<ISerializer, CustomJsonNetSerializer>();
        }
    }
}