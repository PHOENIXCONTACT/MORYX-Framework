using System.ServiceModel;
using Marvin.Container;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Kernel
{
    [Plugin(LifeCycle.Transient, typeof(IVersionService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IncludeExceptionDetailInFaults = true)]
    internal class VersionService : IVersionService
    {
        // Injected by Castle
        public EndpointCollector Collector { get; set; }

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