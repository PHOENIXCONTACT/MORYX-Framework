using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moryx.Communication.Endpoints;
using Moryx.Container;

namespace Moryx.Runtime.Kestrel
{
    [ApiController, Route("endpoints")]
    [Component(LifeCycle.Transient, typeof(Controller))]
    internal class VersionController : Controller
    {
        public EndpointCollector Collector { get; set; }
        
        //[HttpGet]
        //[Produces("application/json")]
        //public Endpoint[] AllEndpoints()
        //{
        //    return Collector.AllEndpoints(HttpContext.Request);
        //}

        //[HttpGet("service/{service}")]
        //[Produces("application/json")]
        //public Endpoint[] FilteredEndpoints(string service)
        //{
        //    return Collector.AllEndpoints(HttpContext.Request)
        //        .Where(e => e.Service == service).ToArray();
        //}

        //[HttpGet("endpoint/{endpoint}")]
        //[Produces("application/json")]
        //public Endpoint GetEndpointConfig(string endpoint)
        //{
        //    return Collector.AllEndpoints(HttpContext.Request)
        //        .FirstOrDefault(e => e.Path == endpoint);
        //}
    }
}
