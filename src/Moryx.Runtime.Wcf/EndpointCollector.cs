// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    internal class EndpointCollector : IEndpointCollector
    {
        private readonly Dictionary<string, ServiceVersionAttribute> _endpoints = new Dictionary<string, ServiceVersionAttribute>();
        private readonly Dictionary<string, ServiceConfig> _services = new Dictionary<string, ServiceConfig>();

        public void AddEndpoint(string endpoint, ServiceVersionAttribute version)
        {
            lock (_endpoints)
            {
                _endpoints[endpoint] = version;
            }
        }

        public void AddService(Type service, ServiceBindingType binding, string serviceUrl, ServiceVersionAttribute version, bool requiresAuthentication)
        {
            lock (_services)
            {
                _services[service.Name] = new ServiceConfig
                {
                    Binding = binding,
                    ServiceUrl = serviceUrl,
                    ServerVersion = version.ServerVersion,
                    MinClientVersion = version.MinClientVersion,
                    RequiresAuthentication = requiresAuthentication
                };
            }
        }

        public void RemoveEndpoint(string endpoint)
        {
            lock (_endpoints)
            {
                _endpoints.Remove(endpoint);
            }
        }

        public void RemoveService(Type service)
        {
            lock (_services)
            {
                _services.Remove(service.Name);
            }
        }

        public ServiceEndpoint[] AllEndpoints()
        {
            lock (_endpoints)
            {
                return _endpoints.Select(pair => new ServiceEndpoint
                    {
                        Endpoint = pair.Key,
                        Version = pair.Value.ServerVersion,
                        MinClientVersion = pair.Value.MinClientVersion
                    }).ToArray();
            }
        }

        public bool ClientSupported(string endpoint, string clientVersion)
        {
            // Get server version
            ServiceVersionAttribute service;
            lock (_endpoints)
            {
                service = _endpoints.ContainsKey(endpoint) ? _endpoints[endpoint] : null;
            }
            if (service == null)
                return false;

            // Parse both versions and compare with each other
            var requiredClient = Version.Parse(service.MinClientVersion);
            var client = Version.Parse(clientVersion);

            return requiredClient.CompareTo(client) <= 0;
        }

        public string GetServerVersion(string endpoint)
        {
            lock (_endpoints)
            {
                return _endpoints.ContainsKey(endpoint) ? _endpoints[endpoint].ServerVersion : null;
            }
        }

        public ServiceConfig GetServiceConfiguration(string service)
        {
            lock (_services)
            {
                return _services.ContainsKey(service) ? _services[service] : null;
            }
        }
    }
}
