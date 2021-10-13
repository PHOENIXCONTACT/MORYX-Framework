// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using System.ServiceModel;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IncludeExceptionDetailInFaults = true)]
    internal class VersionService : IVersionService
    {
        public EndpointCollector Collector { get; set; }

        public Endpoint[] AllEndpoints()
        {
            return Collector.AllEndpoints;
        }

        public Endpoint[] FilteredEndpoints(string service)
        {
            return Collector.AllEndpoints
                .Where(e => e.Service == service).ToArray();
        }

        public Endpoint[] FilterByBinding(string binding)
        {
            return Collector.AllEndpoints
                .Where(e => e.Binding.ToString("G") == binding).ToArray();
        }

        public Endpoint GetEndpointConfig(string endpoint)
        {
            return Collector.AllEndpoints
                .FirstOrDefault(e => e.Path == endpoint);
        }
    }
}
