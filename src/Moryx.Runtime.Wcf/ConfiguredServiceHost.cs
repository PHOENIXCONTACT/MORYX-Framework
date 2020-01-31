// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Moryx.Communication;
using Moryx.Logging;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    internal class ConfiguredServiceHost : IConfiguredServiceHost
    {
        /// <summary>
        /// Di container of parent plugin for component resolution
        /// </summary>
        private readonly IEndpointCollector _collector;
        private readonly PortConfig _portConfig;
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
            IEndpointCollector endpointCollector, PortConfig portConfig)
        {
            _factory = factory;
            _collector = endpointCollector;
            _portConfig = portConfig;

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
            _endpointVersion = CustomAttributeExtensions.GetCustomAttribute<ServiceVersionAttribute>(_type);

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
            binding.OpenTimeout = _portConfig.OpenTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_portConfig.OpenTimeout)
                : TimeSpan.MaxValue;

            binding.CloseTimeout = _portConfig.CloseTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_portConfig.CloseTimeout)
                : TimeSpan.MaxValue;

            binding.SendTimeout = _portConfig.SendTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_portConfig.SendTimeout)
                : TimeSpan.MaxValue;

            binding.ReceiveTimeout = _portConfig.ReceiveTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_portConfig.ReceiveTimeout)
                : TimeSpan.MaxValue;

            // Create endpoint address from config
            var port = config.BindingType == ServiceBindingType.NetTcp ? _portConfig.NetTcpPort : _portConfig.HttpPort;

            // Override binding timeouts if necessary
            if (config is ExtendedHostConfig extendedConfig && extendedConfig.OverrideFrameworkConfig)
            {
                // Override binding timeouts if necessary
                port = extendedConfig.Port;
                binding.OpenTimeout = extendedConfig.OpenTimeout != PortConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.OpenTimeout)
                    : TimeSpan.MaxValue;

                binding.CloseTimeout = extendedConfig.CloseTimeout != PortConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.CloseTimeout)
                    : TimeSpan.MaxValue;

                binding.SendTimeout = extendedConfig.SendTimeout != PortConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.SendTimeout)
                    : TimeSpan.MaxValue;

                binding.ReceiveTimeout = extendedConfig.ReceiveTimeout != PortConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.ReceiveTimeout)
                    : TimeSpan.MaxValue;
            }

            _endpointAddress = $"{protocol}://{_portConfig.Host}:{port}/{config.Endpoint}";

            var endpoint = _service.AddServiceEndpoint(typeof(T), binding, _endpointAddress);

            // Add  behaviors
            endpoint.Behaviors.Add(new CultureBehavior());

            if (config.BindingType == ServiceBindingType.WebHttp)
            {
                endpoint.Behaviors.Add(new WebHttpBehavior());
            }

            if (config.MetadataEnabled)
            {
                var serviceMetadataBehavior = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpGetUrl = new Uri($"http://{_portConfig.Host}:{_portConfig.HttpPort}/Metadata/{config.Endpoint}")
                };
                _service.Description.Behaviors.Add(serviceMetadataBehavior);
            }

            if (config.HelpEnabled)
            {
                var serviceDebugBehavior = _service.Description.Behaviors.Find<ServiceDebugBehavior>();
                serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
                serviceDebugBehavior.HttpHelpPageEnabled = true;
                serviceDebugBehavior.HttpHelpPageUrl = new Uri($"http://{_portConfig.Host}:{_portConfig.HttpPort}/Help/{config.Endpoint}");
            }

            _hostConfig = config;
        }

        #region IConfiguredServiceHost

        /// <inheritdoc />
        public void Start()
        {
            _logger?.Log(LogLevel.Info, "Starting wcf service {0} with version {1}", _endpointAddress,
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
            _collector.RemoveEndpoint(_hostConfig.Endpoint);
            _collector.RemoveService(_type);

            _logger?.Log(LogLevel.Info, "Stopping service {0}", _endpointAddress);

            if (_service != null && _service.State != CommunicationState.Faulted)
            {
                _service.Close();
                _service = null;
            }
        }

        #endregion
    }
}
