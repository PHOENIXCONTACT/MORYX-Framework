// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication;
using Moryx.Tools.Wcf.Tests.Logging;

namespace Moryx.Tools.Wcf.Tests
{
    internal class VersionServiceManagerMock : IVersionServiceManager
    {
        public string Endpoint { get; set; }

        public ServiceBindingType Binding { get; set; }

        public string ServerVersion { get; set; }

        public string ServiceUrl { get; set; }

        public bool EnableVersionService { get; set; }

        public bool IsInitialized { get; private set; }

        public void Initialize(IProxyConfig proxyConfig, string host, int port)
        {
            IsInitialized = true;

            Binding = ServiceBindingType.BasicHttp;
            ServerVersion = "2.0.0.0";
            ServiceUrl = "http://localhost/someservice";
        }

        public Endpoint[] ActiveEndpoints()
        {
            return EnableVersionService ? new Endpoint[]
            {
                new Endpoint
                {
                    Path = Endpoint,
                    Service = nameof(ILogMaintenance),
                    Address = ServiceUrl,
                    Version = ServerVersion,
                    Binding = Binding,
                }
            } : null;
        }

        public Endpoint[] ServiceEndpoints(string service)
        {
            return EnableVersionService ? new Endpoint[]
            {
                new Endpoint
                {
                    Path = Endpoint,
                    Service = service,
                    Address = ServiceUrl,
                    Version = ServerVersion,
                    Binding = Binding,
                }
            } : null;
        }

        public void Dispose()
        {
            IsInitialized = false;
        }
    }
}
