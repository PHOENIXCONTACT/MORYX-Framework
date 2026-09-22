// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders;

/// <summary>
/// Contains information about the update of an operation
/// </summary>
public sealed class OperationUpdate
{
    /// <summary>
    /// OperationSource which should be updates
    /// </summary>
    public IOperationSource OperationSource { get; set; }

    /// <summary>
    /// Sort index of the operation which should be updated
    /// </summary>
    public int? SortIndex { get; set; }

    /// <summary>
    /// Planned start date of the operation which should be updated
    /// </summary>
    public DateTime? PlannedStart { get; set; }

    /// <summary>
    /// Planned end date of the operation which should be updated
    /// </summary>
    public DateTime? PlannedEnd { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.TotalAmount"/>
    /// </summary>
    public int? TotalAmount { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.OverDeliveryAmount"/>
    /// </summary>
    public int? OverDeliveryAmount { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.UnderDeliveryAmount"/>
    /// </summary>
    public int? UnderDeliveryAmount { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.TargetCycleTime"/>
    /// </summary>
    public double? TargetCycleTime { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.TargetStock"/>
    /// </summary>
    public string TargetStock { get; set; }
}
