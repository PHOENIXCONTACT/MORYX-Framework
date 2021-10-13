// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Moryx.Communication;
using Moryx.Communication.Endpoints;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Tools.Wcf;
using Endpoint = Moryx.Tools.Wcf.Endpoint;

namespace Moryx.Runtime.Wcf
{
    internal class WcfEndpointHost : IEndpointHost, ILoggingComponent
    {
        private ServiceHost _service;

        private readonly HostConfig _hostConfig;
        private readonly Type _endpointType;

        private Endpoint _endpoint;

        public ITypedHostFactory TypedHostFactory { get; set; }

        [UseChild("WcfHosting")]
        public IModuleLogger Logger { get; set; }

        public EndpointCollector EndpointCollector { get; set; }

        public PortConfig PortConfig { get; set; }

        public WcfEndpointHost(Type endpoint, object config)
        {
            _endpointType = endpoint;
            _hostConfig = (HostConfig)config;
        }

        private void Setup()
        {
            // Create base address depending on chosen binding
            string protocol;
            switch (_hostConfig.BindingType)
            {
                case ServiceBindingType.NetTcp:
                    protocol = "net.tcp";
                    break;
                default:
                    protocol = "http";
                    break;
            }

            // Create service host
            _service = TypedHostFactory.CreateServiceHost(_endpointType);

            // Configure host
            _service.CloseTimeout = TimeSpan.Zero;

            // Set binding
            Binding binding;
            switch (_hostConfig.BindingType)
            {
                case ServiceBindingType.BasicHttp:
                    binding = BindingFactory.CreateBasicHttpBinding(_hostConfig.RequiresAuthentication);
                    break;
                case ServiceBindingType.NetTcp:
                    binding = BindingFactory.CreateNetTcpBinding(_hostConfig.RequiresAuthentication);
                    break;
                default:
                    binding = BindingFactory.CreateWebHttpBinding();
                    break;
            }

            // Set default timeouts
            binding.OpenTimeout = PortConfig.OpenTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(PortConfig.OpenTimeout)
                : TimeSpan.MaxValue;

            binding.CloseTimeout = PortConfig.CloseTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(PortConfig.CloseTimeout)
                : TimeSpan.MaxValue;

            binding.SendTimeout = PortConfig.SendTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(PortConfig.SendTimeout)
                : TimeSpan.MaxValue;

            binding.ReceiveTimeout = PortConfig.ReceiveTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(PortConfig.ReceiveTimeout)
                : TimeSpan.MaxValue;

            // Create endpoint address from config
            var port = _hostConfig.BindingType == ServiceBindingType.NetTcp ? PortConfig.NetTcpPort : PortConfig.HttpPort;

            // Override binding timeouts if necessary
            if (_hostConfig is ExtendedHostConfig {OverrideFrameworkConfig: true} extendedConfig)
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

            var endpointAddress = $"{protocol}://{PortConfig.Host}:{port}/{_hostConfig.Endpoint}/";

            var endpoint = _service.AddServiceEndpoint(_endpointType, binding, endpointAddress);

            // Add  behaviors
            endpoint.Behaviors.Add(new CultureBehavior());

            if (_hostConfig.BindingType == ServiceBindingType.WebHttp)
            {
                endpoint.Behaviors.Add(new WebHttpBehavior());
                endpoint.Behaviors.Add(new CorsBehaviorAttribute());
            }

            if (_hostConfig.MetadataEnabled)
            {
                var serviceMetadataBehavior = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpGetUrl = new Uri($"http://{PortConfig.Host}:{PortConfig.HttpPort}/Metadata/{_hostConfig.Endpoint}")
                };
                _service.Description.Behaviors.Add(serviceMetadataBehavior);
            }

            if (_hostConfig.HelpEnabled)
            {
                var serviceDebugBehavior = _service.Description.Behaviors.Find<ServiceDebugBehavior>();
                serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
                serviceDebugBehavior.HttpHelpPageEnabled = true;
                serviceDebugBehavior.HttpHelpPageUrl = new Uri($"http://{PortConfig.Host}:{PortConfig.HttpPort}/Help/{_hostConfig.Endpoint}");
            }

            var endpointAttribute = _endpointType.GetCustomAttribute<EndpointAttribute>();
            _endpoint = new Endpoint
            {
                Service = endpointAttribute?.Name ?? _endpointType.Name,
                Path = _hostConfig.Endpoint,
                Address = endpointAddress,
                Binding = _hostConfig.BindingType,
                Version = endpointAttribute?.Version ?? "1.0.0",
                RequiresAuthentication = _hostConfig.RequiresAuthentication
            };
        }

        /// <inheritdoc />
        public void Start()
        {
            Setup();

            Logger?.Log(LogLevel.Info, "Starting wcf service {0} with version {1}", _endpoint.Address,
                _endpoint.Version);

            _service.Open();

            EndpointCollector.AddEndpoint(_endpoint.Address, _endpoint);
        }

        /// <inheritdoc />
        public void Stop()
        {
            EndpointCollector.RemoveEndpoint(_endpoint.Address);

            Logger.Log(LogLevel.Info, "Stopping service {0}", _endpoint.Address);

            if (_service != null && _service.State != CommunicationState.Faulted)
            {
                _service.Close();
                _service = null;
            }
        }
    }
}
