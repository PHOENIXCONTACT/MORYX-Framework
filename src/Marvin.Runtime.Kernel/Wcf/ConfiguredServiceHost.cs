using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Marvin.Logging;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Kernel
{
    internal class ConfiguredServiceHost : IConfiguredServiceHost
    {
        /// <summary>
        /// Di container of parent plugin for component resolution
        /// </summary>
        private readonly EndpointCollector _collector;
        private readonly WcfConfig _wcfConfig;
        private readonly IModuleLogger _logger;


        private ServiceHost _service;
        private readonly ITypedHostFactory _factory;

        /// <summary>
        /// Endpoint information
        /// </summary>
        private HostConfig _hostConfig;
        private Type _type;
        private string _endpointAddress;
        private ServiceVersionAttribute _endpointVersion;

        public ConfiguredServiceHost(ITypedHostFactory factory, IModuleLogger parentLogger,
            EndpointCollector endpointCollector, WcfConfig wcfConfig)
        {
            _factory = factory;
            _collector = endpointCollector;
            _wcfConfig = wcfConfig;

            if (parentLogger != null)
                _logger = parentLogger.GetChild("WcfHosting", GetType());
        }

        /// <summary>
        /// Setup service for given config
        /// </summary>
        /// <param name="config">Config of component</param>
        public void Setup<T>(HostConfig config)
        {
            // Create base address depending on chosen binding
            string protocol;
            switch (config.BindingType)
            {
                case ServiceBindingType.NetTcp:
                    protocol = "net.tcp";
                    break;
                default:
                    protocol = "http";
                    break;
            }

            // Create service host
            _service = _factory.CreateServiceHost<T>();
            _type = typeof (T);
            _endpointVersion = _type.GetCustomAttribute<ServiceVersionAttribute>();

            // Configure host
            _service.CloseTimeout = TimeSpan.Zero;

            // Set binding
            Binding binding;
            switch (config.BindingType)
            {
                case ServiceBindingType.BasicHttp: binding = BindingFactory.CreateBasicHttpBinding(config.RequiresAuthentification);
                    break;
                case ServiceBindingType.NetTcp: binding = BindingFactory.CreateNetTcpBinding(config.RequiresAuthentification);
                    break;
                default: binding = BindingFactory.CreateWebHttpBinding();
                    break;
            }

            // Set default timeouts
            binding.OpenTimeout = _wcfConfig.OpenTimeout != WcfConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_wcfConfig.OpenTimeout)
                : TimeSpan.MaxValue;

            binding.CloseTimeout = _wcfConfig.CloseTimeout != WcfConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_wcfConfig.CloseTimeout)
                : TimeSpan.MaxValue;

            binding.SendTimeout = _wcfConfig.SendTimeout != WcfConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_wcfConfig.SendTimeout)
                : TimeSpan.MaxValue;

            binding.ReceiveTimeout = _wcfConfig.ReceiveTimeout != WcfConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_wcfConfig.ReceiveTimeout)
                : TimeSpan.MaxValue;

            // Create endpoint address from config
            var port = config.BindingType == ServiceBindingType.NetTcp ? _wcfConfig.NetTcpPort : _wcfConfig.HttpPort;

            // Override binding timeouts if necessary
            var extendedConfig = config as ExtendedHostConfig;
            if (extendedConfig != null && extendedConfig.OverrideFrameworkConfig)
            {
                // Override binding timeouts if necessary
                port = extendedConfig.Port;
                binding.OpenTimeout = extendedConfig.OpenTimeout != WcfConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.OpenTimeout)
                    : TimeSpan.MaxValue;

                binding.CloseTimeout = extendedConfig.CloseTimeout != WcfConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.CloseTimeout)
                    : TimeSpan.MaxValue;

                binding.SendTimeout = extendedConfig.SendTimeout != WcfConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.SendTimeout)
                    : TimeSpan.MaxValue;

                binding.ReceiveTimeout = extendedConfig.ReceiveTimeout != WcfConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.ReceiveTimeout)
                    : TimeSpan.MaxValue;
            }

            _endpointAddress = $"{protocol}://{_wcfConfig.Host}:{port}/{config.Endpoint}";

            var endpoint = _service.AddServiceEndpoint(typeof(T), binding, _endpointAddress);
            if (config.BindingType == ServiceBindingType.WebHttp)
            {
                endpoint.Behaviors.Add(new WebHttpBehavior());
            }

            if (config.MetadataEnabled)
            {
                var serviceMetadataBehavior = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpGetUrl = new Uri($"http://{_wcfConfig.Host}:{_wcfConfig.HttpPort}/Metadata/{config.Endpoint}")
                };
                _service.Description.Behaviors.Add(serviceMetadataBehavior);
            }

            if (config.HelpEnabled)
            {
                var serviceDebugBehavior = _service.Description.Behaviors.Find<ServiceDebugBehavior>();
                serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
                serviceDebugBehavior.HttpHelpPageEnabled = true;
                serviceDebugBehavior.HttpHelpPageUrl = new Uri($"http://{_wcfConfig.Host}:{_wcfConfig.HttpPort}/Help/{config.Endpoint}");
            }

            _hostConfig = config;
        }

        #region IConfiguredServiceHost

        /// <inheritdoc />
        public void Start()
        {
            _logger?.LogEntry(LogLevel.Info, "Starting wcf service {0} with version {1}", _endpointAddress,
                (_endpointVersion ?? new ServiceVersionAttribute()).ServerVersion);

            _service.Open();

            if (_endpointVersion != null)
            {
                _collector.AddEndpoint(_hostConfig.Endpoint, _endpointVersion);
                _collector.AddService(_type, _hostConfig.BindingType, _endpointAddress, _endpointVersion, _hostConfig.RequiresAuthentification);
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            //TODO: Distinguish between IDisposable.Dispose() and Stop()
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _collector.RemoveEndpoint(_hostConfig.Endpoint);
            _collector.RemoveService(_type);

            _logger?.LogEntry(LogLevel.Info, "Stopping service {0}", _endpointAddress);

            if (_service != null && _service.State != CommunicationState.Faulted)
            {
                _service.Close();
                _service = null;
            }
        }

        #endregion
    }
}