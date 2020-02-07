// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Common
{
    /// <summary>
    /// Response contract for host information
    /// </summary>
    public class HostInformationResponse
    {
        /// <summary>
        /// Name of the computer. Normally the dns
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Operation system information like the name and version
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string OSInformation { get; set; }

        /// <summary>
        /// Time the system is running
        /// </summary>
        public long UpTime { get; set; }
    }
}
