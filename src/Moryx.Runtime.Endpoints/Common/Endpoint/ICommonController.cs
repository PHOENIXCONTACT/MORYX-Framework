using Microsoft.AspNetCore.Mvc;
using Moryx.Runtime.Endpoints.Common.Endpoint.Response;

namespace Moryx.Runtime.Endpoints.Common.Endpoint
{
    internal interface ICommonController
    {
        /// <summary>
        /// Get the server time.
        /// </summary>
        /// <returns>The current server time.</returns>
        ActionResult<ServerTimeResponse> GetServerTime();

        /// <summary>
        /// Get information about the application
        /// </summary>
        ActionResult<ApplicationInformationResponse> GetApplicationInfo();

        /// <summary>
        /// Get information about the host
        /// </summary>
        /// <returns></returns>
        ActionResult<HostInformationResponse> GetHostInfo();

        /// <summary>
        /// Get information about the system load
        /// </summary>
        /// <returns></returns>
        ActionResult<SystemLoadResponse> GetSystemLoad();

        /// <summary>
        /// Get information about the application load
        /// </summary>
        /// <returns></returns>
        ActionResult<ApplicationLoadResponse> GetApplicationLoad();
    }
}
