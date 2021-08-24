using System;
using System.Net;
using Castle.Windsor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moryx.Communication;
using Moryx.Communication.Endpoints;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Kestrel Http host factory to start up a kestrel server
    /// </summary>
    [InitializableKernelComponent(typeof(IEndpointHosting))]
    internal class KestrelEndpointHosting : IInitializable, IEndpointHosting, IDisposable
    {
        #region Dependencies

        /// <summary>
        /// Config manager
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        #endregion

        #region Fields and Properties
        
        private PortConfig _portConfig;

        private IContainer _hostingContainer;

        private IEndpointHosting _versionHost;

        #endregion

        /// <inheritdoc />
        public void Initialize()
        {
            _portConfig = ConfigManager.GetConfiguration<PortConfig>();

            _hostingContainer = new LocalContainer();
            _hostingContainer.Register<EndpointCollector, EndpointCollector>();
            _hostingContainer.Register<Controller, VersionController>();
            RegisterHostingComponents(_hostingContainer);

            var hostFactory = _hostingContainer.Resolve <IEndpointHostFactory>();
            var host = hostFactory.CreateHost(typeof(VersionController), null);
            host.Start();
        }

        public void ActivateHosting(IContainer container)
        {
            // Fetch collector for endpoit service
            var collector = _hostingContainer.Resolve<EndpointCollector>();
            container.SetInstance(collector);
            // Register everything else
            RegisterHostingComponents(container);
        }

        private void RegisterHostingComponents(IContainer container)
        {
            // TODO: 
            container.SetInstance(_portConfig);
            container.Register<IEndpointHostFactory, KestrelHostFactory>();
        }

        public void Dispose()
        {
            
        }
    }
}
