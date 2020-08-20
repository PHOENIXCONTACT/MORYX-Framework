// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IncludeExceptionDetailInFaults = true)]
    internal class VersionService : IVersionService
    {
        public IEndpointCollector Collector { get; set; }

        public bool ClientSupported(string service, string clientVersion)
        {
            return Collector.ClientSupported(service, clientVersion);
        }

        public string GetServerVersion(string endpoint)
        {
            return Collector.GetServerVersion(endpoint);
        }

        public ServiceEndpoint[] ActiveEndpoints()
        {
            return Collector.AllEndpoints();
        }

        public ServiceConfig GetServiceConfiguration(string service)
        {
            return Collector.GetServiceConfiguration(service);
        }
    }
}
