using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Runtime.Container;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Kernel.Wcf
{
    /// <summary>
    /// Factory to create service hosts and provides hosts for the version service.
    /// </summary>
    [InitializableKernelComponent(typeof(IWcfHostFactory))]
    public class HostFactory : IWcfHostFactory, IInitializable
    {
        /// <summary>
        /// Injected by global container
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        private IContainer _container;
        private WcfConfig _wcfConfig;

        /// <summary>
        /// Initialize the factory and start the version services.
        /// </summary>
        public void Initialize()
        {
            _container = new WcfLocalContainer().ExecuteInstaller(new AutoInstaller(GetType().Assembly));
            StartVersionsServices(ConfigManager);
        }

        private void StartVersionsServices(IConfigManager configManager)
        {
            var factoryConfig = configManager.GetConfiguration<HostFactoryConfig>();
            _wcfConfig = configManager.GetConfiguration<WcfConfig>();
            // Create file to provide access to defect values
            //configManager.SaveConfiguration(_wcfConfig);

            // In minimal core setups with no WCF service this can be disabled
            if(factoryConfig.VersionServiceDisabled)
                return;

            var hostConfig = new HostConfig
            {
                BindingType = ServiceBindingType.BasicHttp,
                Endpoint = "ServiceVersions",
                MetadataEnabled = true
            };

            var collector = _container.Resolve<EndpointCollector>();
            var parentLogger = _container.Resolve<IModuleLogger>();
            var factory = _container.Resolve<ITypedHostFactory>();
            var host = new ConfiguredServiceHost(factory, parentLogger, collector, _wcfConfig);

            host.Setup<IVersionService>(hostConfig);
            host.Start();

            hostConfig = new HostConfig
            {
                Endpoint = "ServiceVersionsWeb",
                MetadataEnabled = true
            };

            host = new ConfiguredServiceHost(factory, parentLogger, collector, _wcfConfig);
            host.Setup<IVersionService>(hostConfig);
            host.Start();
        }

        /// <summary>
        /// Creates a new configurated service host.
        /// </summary>
        /// <typeparam name="T">Type of the host.</typeparam>
        /// <param name="config">Configuration for the host.</param>
        /// <param name="hostFactory">The host facotry which will be used to create a host of type T.</param>
        /// <param name="logger">The logger for the service host.</param>
        /// <returns>A new created configured service host.</returns>
        public IConfiguredServiceHost CreateHost<T>(HostConfig config, ITypedHostFactory hostFactory, IModuleLogger logger)
        {
            var collector = _container.Resolve<EndpointCollector>();

            // Create instance and fill using given container
            var host = new ConfiguredServiceHost(hostFactory, logger, collector, _wcfConfig);
            host.Setup<T>(config);

            return host;
        }
    }
}
