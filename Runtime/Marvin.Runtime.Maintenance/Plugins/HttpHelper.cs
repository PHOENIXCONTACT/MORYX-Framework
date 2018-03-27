using System.Net;
using System.ServiceModel.Web;

namespace Marvin.Runtime.Maintenance.Plugins
{
    internal static class HttpHelper
    {
        public static void SetStatusCode(HttpStatusCode code)
        {
            var ctx = WebOperationContext.Current;
            // ReSharper disable once PossibleNullReferenceException
            ctx.OutgoingResponse.StatusCode = code;
        }
    }
}