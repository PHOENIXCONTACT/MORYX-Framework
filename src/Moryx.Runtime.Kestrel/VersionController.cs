using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moryx.Communication.Endpoints;
using Moryx.Container;

namespace Moryx.Runtime.Kestrel
{
    [ApiController, Route("endpoints")]
    public class VersionController : Controller
    {
        public EndpointCollector Collector { get; set; }

        [HttpGet]
        [Produces("application/json")]
        public Endpoint[] AllEndpoints()
        {
            return Collector.AllEndpoints;
        }

        [HttpGet("service/{service}")]
        [Produces("application/json")]
        public Endpoint[] FilteredEndpoints(string service)
        {
            return Collector.AllEndpoints.Where(e => e.Service == service).ToArray();
        }

        [HttpGet("endpoint/{endpoint}")]
        [Produces("application/json")]
        public Endpoint GetEndpointConfig(string endpoint)
        {
            return Collector.AllEndpoints.FirstOrDefault(e => e.Path == endpoint);
        }
    }
}
