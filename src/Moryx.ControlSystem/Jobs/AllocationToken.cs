// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Jobs;

/// <summary>
/// Delegate for adjusting allocation amounts
/// </summary>
/// <param name="deltaAmount">The amount to adjust (positive to increase, negative to decrease)</param>
/// <returns>A task representing the asynchronous operation</returns>
public delegate Task AdjustAllocationDelegate(int deltaAmount);

/// <summary>
/// Token used to manage allocation changes for preallocated jobs
/// </summary>
/// <remarks>
/// Creates a new allocation token
/// </remarks>
/// <param name="initialAmount">The initially allocated amount</param>
public class AllocationToken(uint initialAmount)
{
    #region Fields and Properties

    private AdjustAllocationDelegate _adjustmentHook;

    /// <summary>
    /// Gets the current status of the allocation token
    /// </summary>
    public AllocationTokenStatus Status
    {
        get => field;
        private set
        {
            field = value;
            StatusChanged?.Invoke(this, value);
        }
    } = AllocationTokenStatus.Created;

    /// <summary>
    /// Gets the current allocated amount
    /// </summary>
    public uint Amount { get; private set; } = initialAmount;

    #endregion

    /// <summary>
    /// Registers a delegate to handle allocation adjustments
    /// </summary>
    /// <param name="adjustmentHook">The delegate to call when adjusting allocations</param>
    /// <exception cref="InvalidOperationException">Thrown if a hook is already registered</exception>
    public void RegisterPreallocationAdjustmentHook(AdjustAllocationDelegate adjustmentHook)
    {
        if (adjustmentHook == null)
        {
            throw new ArgumentNullException(nameof(adjustmentHook), "Adjustment hook cannot be null");
        }

        if (_adjustmentHook != null)
        {
            throw new InvalidOperationException("An adjustment hook is already registered");
        }

        _adjustmentHook = adjustmentHook;
        Status = AllocationTokenStatus.Registered;
    }

    /// <summary>
    /// Adjusts the allocation amount by calling the registered delegate
    /// </summary>
    /// <param name="deltaAmount">The amount to adjust (positive to increase, negative to decrease)</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the allocation is not yet tracked, already locked or if the resulting amount would be <= 0
    /// </exception>
    public async Task AdjustAllocationAsync(int deltaAmount)
    {
        switch (Status)
        {
            case AllocationTokenStatus.Created:
                throw new InvalidOperationException("No adjustment hook has been registered yet");
            case AllocationTokenStatus.Dropped:
                throw new InvalidOperationException("Changes to the allocation are locked.");
        }

        var newAmount = Amount + deltaAmount;
        if (newAmount <= 0)
        {
            throw new InvalidOperationException("The resulting allocation amount would be less than or equal to zero");
        }

        await _adjustmentHook(deltaAmount);
        Amount = (uint)newAmount;
    }

    /// <summary>
    /// Unregisters the adjustment hook and locks the allocation token
    /// </summary>
    public void DropPreallocationAdjustmentHook()
    {
        Status = AllocationTokenStatus.Dropped;
        _adjustmentHook = null;
    }

    /// <summary>
    /// Occurs when the status of the allocation token changes.
    /// </summary>
    /// <remarks>Subscribers can use this event to respond to changes in the allocation token's status, such
    /// as updates to its validity or usage state.</remarks>
    public event EventHandler<AllocationTokenStatus> StatusChanged;
}
