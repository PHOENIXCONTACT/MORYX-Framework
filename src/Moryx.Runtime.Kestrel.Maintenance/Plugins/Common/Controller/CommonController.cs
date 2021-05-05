using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace Moryx.Runtime.Kestrel.Maintenance
{
    /// <summary>
    /// common maintenance controller
    /// </summary>
    [ApiController]
    [Route("common")]
    [ServiceName("ICommonMaintenance")]
    internal class CommonController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// Get the current server time.
        /// </summary>
        /// <returns>The current server time.</returns>
        [HttpGet("time")]
        [Produces("application/json")]
        public ServerTimeResponse GetServerTime()
        {
            return new ServerTimeResponse
            {
                ServerTime = DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
            };
        }

        /// <summary>
        /// Get information about the application
        /// </summary>
        [HttpGet("info/application")]
        [Produces("application/json")]
        public ApplicationInformationResponse GetApplicationInfo()
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

        /// <summary>
        /// Get information about the host
        /// </summary>
        [HttpGet("info/system")]
        [Produces("application/json")]
        public HostInformationResponse GetHostInfo()
        {
            return new HostInformationResponse
            {
                MachineName = Environment.MachineName,
                OSInformation = Environment.OSVersion.ToString(),
                UpTime = Environment.TickCount
            };
        }

        /// <summary>
        /// Get information about the system load
        /// </summary>
        [HttpGet("info/system/load")]
        [Produces("application/json")]
        public SystemLoadResponse GetSystemLoad()
        {
            var physicalMemory = HostHelper.PhysicalMemory();
            var freePhysicalMemory = HostHelper.FreePhysicalMemory();

            return new SystemLoadResponse
            {
                CPULoad = HostHelper.ProcessorTimePercentage(),
                SystemMemoryLoad = (double)(physicalMemory - freePhysicalMemory) / physicalMemory * 100.0
            };
        }

        /// <summary>
        /// Get information about the application load
        /// </summary>
        [HttpGet("info/application/load")]
        [Produces("application/json")]
        public ApplicationLoadResponse GetApplicationLoad()
        {
            var physicalMemory = HostHelper.PhysicalMemory();
            var cpuLoad = 0UL;

            if (!Debugger.IsAttached)
            {
                cpuLoad = HostHelper.ProcessorTimePercentage(Process.GetCurrentProcess().ProcessName);
            }

            return new ApplicationLoadResponse
            {
                CPULoad = cpuLoad,
                SystemMemory = physicalMemory,
                WorkingSet = Environment.WorkingSet
            };
        }
    }
}
