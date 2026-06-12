// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.FactoryMonitor.Endpoints.Models;

public enum InternalOperationClassification
{
    /// <summary>
    /// Classification during the creation
    /// </summary>
    Initial = 0,

    /// <summary>
    /// This operation is loading or reloading operation related information.
    /// </summary>  
    Assigning = 1,

    /// <summary>
    /// The operation failed in the creation process.
    /// </summary>
    Failed = 1 << 1,

    /// <summary>
    /// This operation was declared as aborted and was never started.
    /// </summary>
    Aborted = 1 << 2,

    /// <summary>
    /// Created operation and ready to start the production
    /// </summary>
    Ready = 1 << 3,

    /// <summary>
    /// There is currently a working progress like the production or a reporting
    /// </summary>
    Running = 1 << 4,

    /// <summary>
    /// The operation was interrupted but the production is currently running for the last parts
    /// </summary>
    Interrupting = 1 << 5,

    /// <summary>
    /// The operation reached the current amount or the user has interrupted the operation
    /// </summary>
    Interrupted = 1 << 6,

    /// <summary>
    /// The operation was declared as finished and can not be started again
    /// </summary>
    Completed = 1 << 7,

    /// <summary>
    /// This operation is not finished, but has reached the targeted amount (not equal total amount of the order)
    /// </summary>
    AmountReached = 1 << 8,
}
