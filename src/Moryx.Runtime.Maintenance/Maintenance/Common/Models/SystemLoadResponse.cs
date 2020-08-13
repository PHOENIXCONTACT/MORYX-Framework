// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Common
{
    /// <summary>
    /// Response model for the system load
    /// </summary>
    public class SystemLoadResponse
    {
        /// <summary>
        /// Current cpu load
        /// </summary>
        public ulong CPULoad { get; set; }

        /// <summary>
        /// Current memory usage in percent
        /// </summary>
        public double SystemMemoryLoad { get; set; }
    }
}
