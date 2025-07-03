// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
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
    [InitializableKernelComponent(typeof(IWcfHostFactory), typeof(IEndpointHosting), Name = "WcfHosting")]
    internal class WcfHostFactory : IWcfHostFactory, IInitializable, ILoggingHost, IEndpointHosting
    {
        #region Dependencies

        public IConfigManager ConfigManager { get; set; }

        public ILoggerManagement LoggerManagement { get; set; }

        #endregion

        #region Fields and Properties

        private IContainer _container;
        private PortConfig _portConfig;

        string ILoggingHost.Name => nameof(WcfHostFactory);
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <inheritdoc />
        public void Initialize()
        {
            LoggerManagement.ActivateLogging(this);

            _container = new LocalContainer();
            _portConfig = ConfigManager.GetConfiguration<PortConfig>();

            _container.Register<IVersionService, VersionService>(nameof(VersionService), LifeCycle.Transient);
            _container.Register<EndpointCollector, EndpointCollector>();
            var collector = _container.Resolve<EndpointCollector>();

            _container.Extend<WcfFacility>();
            _container.Register<ITypedHostFactory, TypedHostFactory>();

            var factory = _container.Resolve<ITypedHostFactory>();

            var host = new ConfiguredServiceHost(factory, Logger, collector, _portConfig);
            var hostConfig = new HostConfig
            {
                Endpoint = "endpoints",
                MetadataEnabled = true,
                BindingType = ServiceBindingType.WebHttp
            };
            host.Setup(typeof(IVersionService), hostConfig);
            host.Start();
            if (!string.IsNullOrEmpty(_portConfig.CertificateThumbprint))
            {
                var hostHttps = new ConfiguredServiceHost(factory, Logger, collector, _portConfig);
                var hostHttpsConfig = new HostConfig
                {
                    Endpoint = "endpoints",
                    MetadataEnabled = true,
                    BindingType = ServiceBindingType.WebHttps
                };
                hostHttps.Setup(typeof(IVersionService), hostHttpsConfig);
                hostHttps.Start();
            }
        }

        /// <inheritdoc />
        public IConfiguredServiceHost CreateHost<TContract>(HostConfig config, ITypedHostFactory hostFactory, IModuleLogger logger) =>
            CreateHost(typeof(TContract), config, hostFactory, logger);

        public IConfiguredServiceHost CreateHost(Type contract, HostConfig config, ITypedHostFactory hostFactory,
            IModuleLogger logger)
        {
            var collector = _container.Resolve<EndpointCollector>();

            // Create instance and fill using given container
            var host = new ConfiguredServiceHost(hostFactory, logger, collector, _portConfig);
            host.Setup(contract, config);

            return host;
        }

        public void ActivateHosting(IContainer container)
        {
            container.RegisterWcf(this);
        }
    }
}