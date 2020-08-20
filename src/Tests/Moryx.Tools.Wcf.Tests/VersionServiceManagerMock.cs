// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication;

namespace Moryx.Tools.Wcf.Tests
{
    internal class VersionServiceManagerMock : IVersionServiceManager
    {
        public BindingType Binding { get; set; }
        public string MinClientVersion { get; set; }
        public string ServerVersion { get; set; }
        public string ServiceUrl { get; set; }
        public bool EnableVersionService { get; set; }

        public bool IsInitialized { get; private set; }

        public void Initialize(IProxyConfig proxyConfig, string host, int port)
        {
            IsInitialized = true;

            Binding = BindingType.BasicHttp;
            MinClientVersion = "2.0.0.0";
            ServerVersion = "1.0.0.0";
            ServiceUrl = "http://blah.fasel";
            EnableVersionService = true;
        }

        public bool CheckClientSupport(string service, string clientVersion)
        {
            // Parse both versions and compare with each other
            var requiredClient = Version.Parse(MinClientVersion);
            var client = Version.Parse(clientVersion);

            return EnableVersionService && requiredClient.CompareTo(client) <= 0;
        }


        public string GetServerVersion(string endpoint)
        {
            return EnableVersionService ? ServerVersion : null;
        }


        public ServiceConfiguration GetServiceConfiguration(string service)
        {
            return EnableVersionService ? new ServiceConfiguration
            {
                BindingType = Binding,
                MinClientVersion = MinClientVersion,
                ServerVersion = ServerVersion,
                ServiceUrl = ServiceUrl
            } : null;
        }

        public bool Match(IClientConfig config, string version)
        {
            var minServerVersion = Version.Parse(config.MinServerVersion);
            var serverVersion = Version.Parse(version);

            var serverSupport = CheckClientSupport(config.Endpoint, config.ClientVersion);

            return minServerVersion <= serverVersion && serverSupport;
        }

        public bool Match(WcfClientInfo clientInfo, ServiceConfiguration sericeConfiguration)
        {
            var minServerVersion = Version.Parse(clientInfo.MinServerVersion);
            var serverVersion = Version.Parse(sericeConfiguration.ServerVersion);

            var minClientVersion = Version.Parse(sericeConfiguration.MinClientVersion);
            var clientVersion = Version.Parse(clientInfo.ClientVersion);

            return minServerVersion <= serverVersion && minClientVersion <= clientVersion;
        }

        public void Dispose()
        {
            IsInitialized = false;
        }
    }
}
