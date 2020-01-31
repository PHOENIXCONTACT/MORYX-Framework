// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Facilities.WcfIntegration;
using Moryx.Communication;
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
    [InitializableKernelComponent(typeof(IWcfHostFactory))]
    internal class WcfHostFactory : IWcfHostFactory, IInitializable, ILoggingHost
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
            var factoryConfig = ConfigManager.GetConfiguration<HostFactoryConfig>();
            _portConfig = ConfigManager.GetConfiguration<PortConfig>();

            // In minimal core setups with no WCF service this can be disabled
            if (factoryConfig.VersionServiceDisabled)
                return;

            var hostConfig = new HostConfig
            {
                BindingType = ServiceBindingType.BasicHttp,
                Endpoint = "ServiceVersions",
                MetadataEnabled = true
            };

            _container.Register<IVersionService, VersionService>(nameof(VersionService), LifeCycle.Transient);
            _container.Register<IEndpointCollector, EndpointCollector>();
            var collector = _container.Resolve<IEndpointCollector>();

            _container.Extend<WcfFacility>();
            _container.Register<ITypedHostFactory, TypedHostFactory>();

            var factory = _container.Resolve<ITypedHostFactory>();
            var host = new ConfiguredServiceHost(factory, Logger, collector, _portConfig);

            host.Setup<IVersionService>(hostConfig);
            host.Start();

            hostConfig = new HostConfig
            {
                Endpoint = "ServiceVersionsWeb",
                MetadataEnabled = true
            };

            host = new ConfiguredServiceHost(factory, Logger, collector, _portConfig);
            host.Setup<IVersionService>(hostConfig);
            host.Start();
        }

        /// <inheritdoc />
        public IConfiguredServiceHost CreateHost<T>(HostConfig config, ITypedHostFactory hostFactory, IModuleLogger logger)
        {
            var collector = _container.Resolve<IEndpointCollector>();

            // Create instance and fill using given container
            var host = new ConfiguredServiceHost(hostFactory, logger, collector, _portConfig);
            host.Setup<T>(config);

            return host;
        }
    }
}