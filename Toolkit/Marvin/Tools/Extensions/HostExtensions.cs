using System;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace Marvin.Tools
{
    /// <summary>
    /// Extensions to retrieve system information
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Returns the amount of physical memory installed
        /// </summary>
        /// <returns></returns>
        public static ulong PhysicalMemory()
        {
            var mc = new ManagementClass("Win32_ComputerSystem");
            var moc = mc.GetInstances();
            return (from ManagementObject item in moc select Convert.ToUInt64(item.Properties["TotalPhysicalMemory"].Value)).FirstOrDefault();
        }

        /// <summary>
        /// Returns the current free physical memory
        /// </summary>
        /// <returns></returns>
        public static ulong FreePhysicalMemory()
        {
            var mc = new ManagementClass("Win32_OperatingSystem");
            var moc = mc.GetInstances();
            return (from ManagementObject item in moc select Convert.ToUInt64(item.Properties["FreePhysicalMemory"].Value) * 1024).FirstOrDefault();
        }

        /// <summary>
        /// Returns the processor time of a specific process
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static ulong ProcessorTimePercentage(string processName = "_Total")
        {
            var processorTime = new ManagementObjectSearcher($"SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='{processName}'")
                .Get()
                .Cast<ManagementObject>().Sum(o => (long)(ulong)o.Properties["PercentProcessorTime"].Value);

            return (ulong)processorTime;
        }
    }
}
