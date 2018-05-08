using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Marvin.Container;

namespace Marvin.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Wcf service implementations for the common maintenance.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    [Plugin(LifeCycle.Transient, typeof(ICommonMaintenance))]
    public class CommonMaintenance : ICommonMaintenance
    {
        /// <summary>
        /// Get the current server time.
        /// </summary>
        /// <returns>The current server time.</returns>
        public ServerTimeResponse GetServerTime()
        {
            return new ServerTimeResponse
            {
                ServerTime = DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
            };
        }

        /// <inheritdoc />
        public ApplicationInformationResponse GetApplicationInfo()
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(true);
            var assemblyProductAttribute = attributes.FirstOrDefault(a => a is AssemblyProductAttribute) as AssemblyProductAttribute;
            var assemblyVersionAttribute = attributes.FirstOrDefault(a => a is AssemblyFileVersionAttribute) as AssemblyFileVersionAttribute;
            var assemblyInfoVersionAttribute = attributes.FirstOrDefault(a => a is AssemblyInformationalVersionAttribute) as AssemblyInformationalVersionAttribute;
            var assemblyDescriptionAttribute = attributes.FirstOrDefault(a => a is AssemblyDescriptionAttribute) as AssemblyDescriptionAttribute;

            return new ApplicationInformationResponse
            {
                AssemblyProduct = assemblyProductAttribute?.Product,
                AssemblyVersion = assemblyVersionAttribute?.Version,
                AssemblyInformationalVersion = assemblyInfoVersionAttribute?.InformationalVersion,
                AssemblyDescription = assemblyDescriptionAttribute?.Description
            };
        }

        /// <inheritdoc />
        public HostInformationResponse GetHostInfo()
        {
            return new HostInformationResponse
            {
                MachineName = Environment.MachineName,
                OSInformation = Environment.OSVersion.ToString(),
                UpTime = Environment.TickCount
            };
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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