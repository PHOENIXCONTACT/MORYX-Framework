// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Enum flags: Notify - Reincarnate
    /// </summary>
    [Flags]
    public enum FailureBehaviour
    {
        /// <summary>
        /// Module is stopped when an exception occurs
        /// </summary>
        Stop = 0x00,

        /// <summary>
        /// Module is stopped and failure processed by notification components
        /// </summary>
        StopAndNotify = 0x02,

        /// <summary>
        /// Module is reincarnated
        /// </summary>
        Reincarnate = 0x01,

        /// <summary>
        /// Module is reincarnated and the failure notified
        /// </summary>
        ReincarnateAndNotify = 0x03,
    }
}
