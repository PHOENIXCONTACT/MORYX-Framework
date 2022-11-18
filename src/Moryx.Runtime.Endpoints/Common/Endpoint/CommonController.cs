using Microsoft.AspNetCore.Mvc;
using Moryx.Runtime.Endpoints.Common.Endpoint.Response;
using System;
using System.Globalization;
using System.Reflection;

namespace Moryx.Runtime.Endpoints.Common.Endpoint
{
    [ApiController]
    [Route("common")]
    [Produces("application/json")]
    public class CommonController : ControllerBase
    {
        public CommonController()
        {
        }

        [HttpGet]
        [Route("time")]
        public ActionResult<ServerTimeResponse> GetServerTime()
        {
            return new ServerTimeResponse
            {
                ServerTime = DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
            };
        }

        [HttpGet("info/application")]
        public ActionResult<ApplicationInformationResponse> GetApplicationInfo()
        {
            var startAssembly = Assembly.GetEntryAssembly();
            var version = new Version(startAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? "1.0.0");
            return new ApplicationInformationResponse
            {
                AssemblyProduct = startAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "MORYX Application",
                AssemblyVersion = version.ToString(3),
                AssemblyInformationalVersion = version.ToString(3),
                AssemblyDescription = startAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "No Description provided!",
            };
        }

        [HttpGet("info/system")]
        public ActionResult<HostInformationResponse> GetHostInfo()
        {
            return new HostInformationResponse
            {
                MachineName = Environment.MachineName,
                OSInformation = Environment.OSVersion.ToString(),
                UpTime = Environment.TickCount
            };
        }
    }
}
