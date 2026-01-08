// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
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
    }
}
