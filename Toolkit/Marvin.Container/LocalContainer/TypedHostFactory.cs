using System;
using System.ServiceModel;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel;
using Marvin.Tools.Wcf;

namespace Marvin.Container
{
    internal class TypedHostFactory : DefaultServiceHostFactory, ITypedHostFactory
    {
        private readonly IKernel _kernel;
        public TypedHostFactory(IKernel kernel)
            : base(kernel)
        {
            _kernel = kernel;
        }

        public ServiceHost CreateServiceHost<T>()
        {
            // Get implementation type of service
            var handler = _kernel.GetHandler(typeof (T));
            var compModel = handler.ComponentModel;
            return base.CreateServiceHost(compModel.Implementation, new Uri[0]);
        }
    }
}
