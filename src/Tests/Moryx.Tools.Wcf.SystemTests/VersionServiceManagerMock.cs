// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
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

        public VersionServiceManagerMock()
        {
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

        public Task<Endpoint[]> ActiveEndpointsAsync()
        {
            return Task.FromResult(ActiveEndpoints());
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

        public Task<Endpoint[]> ServiceEndpointsAsync(string service)
        {
            return Task.FromResult(ServiceEndpoints(service));
        }

        public void Dispose()
        {
        }
    }
}
