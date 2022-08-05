// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Endpoints.Common
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
            return 42; // TODO: Replace in Core 4
        }

        /// <summary>
        /// Returns the current free physical memory
        /// </summary>
        /// <returns></returns>
        public static ulong FreePhysicalMemory()
        {
            return 32; // TODO: Replace in Core 4
        }

        /// <summary>
        /// Returns the processor time of a specific process
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static ulong ProcessorTimePercentage(string processName = "_Total")
        {
            return 0; // TODO: Replace in Core 4
        }
    }
}
