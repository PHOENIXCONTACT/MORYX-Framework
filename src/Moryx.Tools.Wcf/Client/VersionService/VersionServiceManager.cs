// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ServiceModel;
using Moryx.Communication;
using Moryx.Configuration;

namespace Moryx.Tools.Wcf
{
    internal class VersionServiceManager : IVersionServiceManager
    {
        private const string ServiceName = "ServiceVersions";

        #region Fields and Properties

        public bool IsInitialized { get; private set; }

        private IVersionService _versionService;

        #endregion

        ///
        public void Initialize(IProxyConfig proxyConfig, string host, int port)
        {
            if (IsInitialized)
                return;

            // Create version service
            var binding = new BasicHttpBinding
            {
                SendTimeout = new TimeSpan(0, 0, 2),
                OpenTimeout = new TimeSpan(0, 0, 2)
            };

            //Set proxy
            SetProxyOnBinding(proxyConfig, binding);

            var url = $@"http://{host}:{port}/{ServiceName}";
            var endpoint = new EndpointAddress(url);

            var channelFactory = new ChannelFactory<IVersionService>(binding);
            channelFactory.Endpoint.Behaviors.Add(new CultureBehavior());

            _versionService = channelFactory.CreateChannel(endpoint);

            IsInitialized = true;
        }

        ///
        public void Dispose()
        {
            if (!IsInitialized)
                return;

            _versionService = null;
            IsInitialized = false;
        }

        ///
        public bool CheckClientSupport(string service, string clientVersion)
        {
            return _versionService.ClientSupported(service, clientVersion);
        }

        ///
        public string GetServerVersion(string endpoint)
        {
            return _versionService.GetServerVersion(endpoint);
        }

        ///
        public ServiceConfiguration GetServiceConfiguration(string service)
        {
            var serviceConfig = _versionService.GetServiceConfiguration(service);
            return serviceConfig == null ? null : new ServiceConfiguration(serviceConfig);
        }

        ///
        public bool Match(IClientConfig config, string version)
        {
            var minServerVersion = Version.Parse(config.MinServerVersion);
            var serverVersion = Version.Parse(version);

            var serverSupport = CheckClientSupport(config.Endpoint, config.ClientVersion);

            return minServerVersion <= serverVersion && serverSupport;
        }

        ///
        public bool Match(WcfClientInfo clientInfo, ServiceConfiguration sericeConfiguration)
        {
            var minServerVersion = Version.Parse(clientInfo.MinServerVersion);
            var serverVersion = Version.Parse(sericeConfiguration.ServerVersion);

            var minClientVersion = Version.Parse(sericeConfiguration.MinClientVersion);
            var clientVersion = Version.Parse(clientInfo.ClientVersion);

            return minServerVersion <= serverVersion && minClientVersion <= clientVersion;
        }

        ///
        private static void SetProxyOnBinding(IProxyConfig proxyConfig, BasicHttpBinding binding)
        {
            if (proxyConfig == null || !proxyConfig.EnableProxy)
                return;

            if (proxyConfig.UseDefaultWebProxy)
            {
                binding.UseDefaultWebProxy = true;
            }
            else if (!string.IsNullOrEmpty(proxyConfig.Address) && proxyConfig.Port != 0)
            {
                binding.ProxyAddress = new Uri($"http://{proxyConfig.Address}:{proxyConfig.Port}");
            }
        }
    }
}
