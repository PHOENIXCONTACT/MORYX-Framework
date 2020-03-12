// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
            var currentPlatform = Platform.Current;
            return new ApplicationInformationResponse
            {
                AssemblyProduct = currentPlatform.ProductName,
                AssemblyVersion = currentPlatform.ProductVersion.ToString(3),
                AssemblyInformationalVersion = currentPlatform.ProductVersion.ToString(3),
                AssemblyDescription = currentPlatform.ProductDescription
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
