// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production;

/// <summary>
/// Implementation of a preallocated production job that supports dynamic allocation adjustments before starting the job
/// </summary>
/// <inheritdoc />
[DebuggerDisplay(nameof(PreallocatedJobData) + " <Id: {" + nameof(Id) + "}, State: {" + nameof(State) + "}, Allocation Status: {" + nameof(AllocationToken.Status) + "}>")]
[Component(LifeCycle.Transient, typeof(IPreallocatedJobData))]
internal sealed class PreallocatedJobData(ProductionRecipe recipe, int amount, AllocationToken token) :
    ProductionJobData(recipe, amount),
    IPreallocatedJobData
{
    /// <inheritdoc />
    public AllocationToken AllocationToken { get; private set; } = token;

    /// <inheritdoc />
    public void AdjustAmount(int newAmount)
    {
        if (AllocationToken is null || AllocationToken.Status == AllocationTokenStatus.Dropped)
        {
            Logger.LogError("Tried adjusting the amount on a frozen preallocated job {id}", Id);
            return;
        }

        Amount = newAmount;
    }

    /// <inheritdoc />
    public void FreezePreallocation()
    {
        AllocationToken.DropPreallocationAdjustmentHook();
        AllocationToken = null;

        Logger.Log(LogLevel.Debug, "Froze preallocated job {id} on amount {amount}", Id, Amount);
    }
}
