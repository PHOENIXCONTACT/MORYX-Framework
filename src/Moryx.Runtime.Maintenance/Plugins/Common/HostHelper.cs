// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#if HAVE_SYSTEM_MANAGEMENT
using System;
using System.Linq;
using System.Management;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Extensions to retrieve system information
    /// </summary>
    internal static class HostHelper
    {
        /// <summary>
        /// Returns the amount of physical memory installed
        /// </summary>
        /// <returns></returns>
        public static ulong PhysicalMemory()
        {
#if HAVE_SYSTEM_MANAGEMENT
            var mc = new ManagementClass("Win32_ComputerSystem");
            var moc = mc.GetInstances();
            return (from ManagementObject item in moc select Convert.ToUInt64(item.Properties["TotalPhysicalMemory"].Value)).FirstOrDefault();
#else
            return 42; // TODO: Replace in Core 4
#endif
        }

        /// <summary>
        /// Returns the current free physical memory
        /// </summary>
        /// <returns></returns>
        public static ulong FreePhysicalMemory()
        {
#if HAVE_SYSTEM_MANAGEMENT
            var mc = new ManagementClass("Win32_OperatingSystem");
            var moc = mc.GetInstances();
            return (from ManagementObject item in moc select Convert.ToUInt64(item.Properties["FreePhysicalMemory"].Value) * 1024).FirstOrDefault();
#else
            return 32; // TODO: Replace in Core 4
#endif
        }

        /// <summary>
        /// Returns the processor time of a specific process
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static ulong ProcessorTimePercentage(string processName = "_Total")
        {
#if HAVE_SYSTEM_MANAGEMENT
            var processorTime = new ManagementObjectSearcher($"SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='{processName}'")
                .Get()
                .Cast<ManagementObject>().Sum(o => (long)(ulong)o.Properties["PercentProcessorTime"].Value);

            return (ulong)processorTime;
#else

            return 0; // TODO: Replace in Core 4
#endif
        }
    }
}
