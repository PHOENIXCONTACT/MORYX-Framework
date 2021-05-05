using System.Linq;
using Hardware.Info;

namespace Moryx.Runtime.Kestrel.Maintenance
{
    /// <summary>
    /// Extensions to retrieve system information
    /// </summary>
    internal static class HostHelper
    {
        private static readonly HardwareInfo HardwareInfo = new HardwareInfo();
        
        /// <summary>
        /// Returns the amount of physical memory installed
        /// </summary>
        /// <returns></returns>
        public static ulong PhysicalMemory()
        {
            HardwareInfo.RefreshMemoryStatus();
            return HardwareInfo.MemoryStatus.TotalPhysical;
        }

        /// <summary>
        /// Returns the current free physical memory
        /// </summary>
        /// <returns></returns>
        public static ulong FreePhysicalMemory()
        {
            HardwareInfo.RefreshMemoryStatus();
            return HardwareInfo.MemoryStatus.AvailablePhysical;
        }

        /// <summary>
        /// Returns the processor time of a specific process
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static ulong ProcessorTimePercentage(string processName = "_Total")
        {
            HardwareInfo.RefreshCPUList();
            return (ulong)(HardwareInfo.CpuList.Sum(c => (decimal)c.PercentProcessorTime) / HardwareInfo.CpuList.Count);
        }
    }
}
