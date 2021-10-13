// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Facilities.WcfIntegration;
using Moryx.Communication;
using Moryx.Communication.Endpoints;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    /// <summary>
    /// Factory to create service hosts and provides hosts for the version service.
    /// </summary>
    [InitializableKernelComponent(typeof(IEndpointHosting), Name = "WcfHosting")]
    internal class WcfEndpointHosting : IInitializable, ILoggingHost, IEndpointHosting
    {
        #region Dependencies

        public IConfigManager ConfigManager { get; set; }

        public ILoggerManagement LoggerManagement { get; set; }

        #endregion

        #region Fields and Properties

        private IContainer _hostingContainer;

        string ILoggingHost.Name => nameof(WcfEndpointHosting);
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <inheritdoc />
        public void Initialize()
        {
            LoggerManagement.ActivateLogging(this);

            var portConfig = ConfigManager.GetConfiguration<PortConfig>();

            _hostingContainer = new LocalContainer();
            _hostingContainer.SetInstance(Logger);
            _hostingContainer.SetInstance(portConfig);

            _hostingContainer.Extend<WcfFacility>();
            _hostingContainer.Register<ITypedHostFactory, TypedHostFactory>();

            _hostingContainer.Register<IEndpointHostFactory>();
            _hostingContainer.Register<IEndpointHost, WcfEndpointHost>("WcfHost", LifeCycle.Transient);

            _hostingContainer.Register<IVersionService, VersionService>(nameof(VersionService), LifeCycle.Transient);
            _hostingContainer.Register<EndpointCollector, EndpointCollector>();

            var endpointHostFactory = _hostingContainer.Resolve<IEndpointHostFactory>();
            var hostConfig = new HostConfig
            {
                Endpoint = "endpoints",
                MetadataEnabled = true
            };

            var host = endpointHostFactory.CreateHost(typeof(IVersionService), hostConfig);
            host.Start();
        }

        public void ActivateHosting(IContainer container)
        {
            var portConfig = ConfigManager.GetConfiguration<PortConfig>();
            container.SetInstance(portConfig);

            container.Extend<WcfFacility>();
            container.Register<ITypedHostFactory, TypedHostFactory>();

            // Register endpoint collector
            var endpointCollector = _hostingContainer.Resolve<EndpointCollector>();
            container.SetInstance(endpointCollector);

            // Register factory and host
            container.Register<IEndpointHost, WcfEndpointHost>("WcfHost", LifeCycle.Transient);

            container.Register<IEndpointHostFactory>();

            container.SetInstance(this);
        }
    }
}