using Microsoft.AspNetCore.Http;

namespace Moryx.Runtime.Kestrel
{
    internal interface IEndpointCollector
    {
        Endpoint[] AllEndpoints(HttpRequest request);
    }
}
