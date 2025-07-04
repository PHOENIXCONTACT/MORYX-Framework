﻿// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Orders
{
    /// <summary>
    /// Enum providing the classification of the internal state machine of the operation for an external representation
    /// The lowest 8 bits define the state of the operation. The following 16 bits contain flags for the executable 
    /// actions on the operation and the trailing 8 bits overall classification information.
    /// Bit:    31 - 24  |  23 - 8  |  7 - 0  |
    /// Flag:     Type   |   Usage  |  State  |
    /// </summary>
    [Flags]
    public enum OperationClassification
    {
        /// <summary>
        /// Classification during the creation
        /// </summary>
        Initial = 0,

        /// <summary>
        /// Created operation and ready to start the production
        /// </summary>
        Ready = 1,

        /// <summary>
        /// There is currently a working progress like the production or a reporting
        /// </summary>
        Running = 2,

        /// <summary>
        /// The operation was interrupted but the production is currently running for the last parts
        /// </summary>
        Interrupting = 3,

        /// <summary>
        /// The operation reached the current amount or the user has interrupted the operation
        /// </summary>
        Interrupted = 4,

        /// <summary>
        /// The operation was declared as finished and can not be started again
        /// </summary>
        Completed = 5,

        /// <summary>
        /// This operation was declared as aborted and was never started.
        /// </summary>
        Aborted = 6,

        /// <summary>
        /// This operation is loading or reloading operation related information.
        /// </summary>
        Loading = 7,

        /// <summary>
        /// The operation failed in the creation process.
        /// </summary>
        Failed = 11,

        /// <summary>
        /// Flag if the operation can be reloaded
        /// </summary>
        CanReload = (1 << 8),

        /// <summary>
        /// Flag if the operation can begin
        /// </summary>
        CanBegin = (1 << 10),

        /// <summary>
        /// Flag if the operation can be interrupted
        /// </summary>
        CanInterrupt = (1 << 12),

        /// <summary>
        /// Flag if the operation can be reported
        /// </summary>
        CanReport = (1 << 16),

        /// <summary>
        /// Flag if the operation can be adviced
        /// </summary>
        CanAdvice = (1 << 20),

        /// <summary>
        /// Flag if the operation is running but has no process
        /// </summary>
        IsAmountReached = (1 << 22),
    }
}
