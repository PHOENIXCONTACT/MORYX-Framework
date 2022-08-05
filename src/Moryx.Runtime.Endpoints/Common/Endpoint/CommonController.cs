using Microsoft.AspNetCore.Mvc;
using Moryx.Runtime.Endpoints.Common.Endpoint.Response;
using System;
using System.Diagnostics;
using System.Globalization;

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
            var currentPlatform = Platform.Current;
            return new ApplicationInformationResponse
            {
                AssemblyProduct = currentPlatform.ProductName,
                AssemblyVersion = currentPlatform.ProductVersion.ToString(3),
                AssemblyInformationalVersion = currentPlatform.ProductVersion.ToString(3),
                AssemblyDescription = currentPlatform.ProductDescription
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

        [HttpGet("info/system/load")]
        public ActionResult<SystemLoadResponse> GetSystemLoad()
        {
            var physicalMemory = HostHelper.PhysicalMemory();
            var freePhysicalMemory = HostHelper.FreePhysicalMemory();

            return new SystemLoadResponse
            {
                CPULoad = HostHelper.ProcessorTimePercentage(),
                SystemMemoryLoad = (double)(physicalMemory - freePhysicalMemory) / physicalMemory * 100.0
            };
        }

        [HttpGet("info/application/load")]
        public ActionResult<ApplicationLoadResponse> GetApplicationLoad()
        {
            var physicalMemory = HostHelper.PhysicalMemory();
            var cpuLoad = 0UL;

            if (!Debugger.IsAttached)
                cpuLoad = HostHelper.ProcessorTimePercentage(Process.GetCurrentProcess().ProcessName);

            return new ApplicationLoadResponse
            {
                CPULoad = cpuLoad,
                SystemMemory = physicalMemory,
                WorkingSet = Environment.WorkingSet
            };
        }
    }
}
