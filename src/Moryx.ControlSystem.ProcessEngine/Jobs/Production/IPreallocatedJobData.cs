// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production;

/// <summary>
/// Interface for preallocated production job data that supports dynamic allocation adjustments
/// </summary>
internal interface IPreallocatedJobData : IProductionJobData
{
    /// <summary>
    /// Gets the allocation token for this preallocated job
    /// </summary>
    AllocationToken AllocationToken { get; }

    /// <summary>
    /// Adjusts the target amount of the preallocated job
    /// </summary>
    /// <param name="newAmount">The new target amount</param>
    void AdjustAmount(int newAmount);

    /// <summary>
    /// Freezes this <see cref="IProductionJobData"/> so that no further preallocation adjustments are possible.
    /// </summary>
    void FreezePreallocation();
}
