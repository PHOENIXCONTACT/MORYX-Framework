// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;
using System.Globalization;
using Moryx.Container;
#if USE_WCF
using System.ServiceModel;
#else
using Microsoft.AspNetCore.Mvc;
using Moryx.Communication.Endpoints;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Wcf service implementations for the common maintenance.
    /// </summary>
    [Plugin(LifeCycle.Transient, typeof(ICommonMaintenance))]
#if USE_WCF
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    public class CommonMaintenance : ICommonMaintenance
#else
    [ApiController, Route(Endpoint), Produces("application/json")]
    [Endpoint(Name = nameof(ICommonMaintenance), Version = "3.0.0")]
    public class CommonMaintenance : Controller, ICommonMaintenance
#endif
    {
        internal const string Endpoint = "common";

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet("time")]
#endif
        public ServerTimeResponse GetServerTime()
        {
            return new ServerTimeResponse
            {
                ServerTime = DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
            };
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet("info/application")]
#endif
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
#if !USE_WCF
        [HttpGet("info/system")]
#endif
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
#if !USE_WCF
        [HttpGet("info/system/load")]
#endif
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
#if !USE_WCF
        [HttpGet("info/application/load")]
#endif
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
