// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Common
{
    /// <summary>
    /// Contract for application load response
    /// </summary>
    public class ApplicationLoadResponse
    {
        /// <summary>
        /// Current processor time in percent
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public ulong CPULoad { get; set; }

        /// <summary>
        /// Total physical memory of the system
        /// </summary>
        public ulong SystemMemory { get; set; }

        /// <summary>
        /// Gets the amount of physical memory mapped to the process context.
        /// </summary>
        public long WorkingSet { get; set; }
    }
}
