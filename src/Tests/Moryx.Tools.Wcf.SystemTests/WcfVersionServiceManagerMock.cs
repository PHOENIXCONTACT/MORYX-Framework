// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using System.Threading.Tasks;
using Moryx.Communication.Endpoints;
using Moryx.Tools.Wcf.Tests.Logging;

namespace Moryx.Tools.Wcf.Tests
{
    internal class WcfVersionServiceManagerMock : IVersionServiceManager
    {
        public string Endpoint { get; set; }

        public ServiceBindingType Binding { get; set; }

        public string ServerVersion { get; set; }

        public string ServiceUrl { get; set; }

        public bool EnableVersionService { get; set; }

        public WcfVersionServiceManagerMock()
        {
            Binding = ServiceBindingType.BasicHttp;
            ServerVersion = "2.0.0.0";
            ServiceUrl = "http://localhost/someservice";
        }

        public Communication.Endpoints.Endpoint[] ActiveEndpoints()
        {
            var endpoints = new[]
            {
                new Endpoint
                {
                    Path = Endpoint,
                    Service = nameof(ILogMaintenance),
                    Address = ServiceUrl,
                    Version = ServerVersion,
                    Binding = Binding,
                }
            };

            return EnableVersionService ? endpoints.ToArray<Communication.Endpoints.Endpoint>() : null;
        }

        public Task<Communication.Endpoints.Endpoint[]> ActiveEndpointsAsync()
        {
            return Task.FromResult(ActiveEndpoints());
        }

        public Communication.Endpoints.Endpoint[] ServiceEndpoints(string service)
        {
            var endpoints = new[]
            {
                new Endpoint
                {
                    Path = Endpoint,
                    Service = service,
                    Address = ServiceUrl,
                    Version = ServerVersion,
                    Binding = Binding,
                }
            };

            return EnableVersionService ? endpoints.ToArray<Communication.Endpoints.Endpoint>() : null;
        }

        public Task<Communication.Endpoints.Endpoint[]> ServiceEndpointsAsync(string service)
        {
            return Task.FromResult(ServiceEndpoints(service));
        }

        public void Dispose()
        {
        }
    }
}
