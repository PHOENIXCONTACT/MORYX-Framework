// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;
using System.Globalization;
using Moryx.Container;
using Moryx.Runtime.Maintenance.Common;
using Nancy;

namespace Moryx.Runtime.Maintenance
{
    /// <summary>
    /// Wcf service implementations for the common maintenance.
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(INancyModule), typeof(CommonMaintenance))]
    internal sealed class CommonMaintenance : NancyModule
    {
        public CommonMaintenance() : base("common")
        {
            Get("/serverTime", args => Response.AsJson(GetServerTime()));
            Get("/applicationInfo", args => Response.AsJson(GetApplicationInfo()));
            Get("/hostInfo", args => Response.AsJson(GetHostInfo()));
            Get("/systemLoad", args => Response.AsJson(GetSystemLoad()));
            Get("/applicationLoad", args => Response.AsJson(GetApplicationLoad()));
        }

        public ServerTimeResponse GetServerTime()
        {
            return new ServerTimeResponse
            {
                ServerTime = DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
            };
        }

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

        public HostInformationResponse GetHostInfo()
        {
            return new HostInformationResponse
            {
                MachineName = Environment.MachineName,
                OSInformation = Environment.OSVersion.ToString(),
                UpTime = Environment.TickCount
            };
        }

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
