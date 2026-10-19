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

    /// <inheritdoc cref="Operation.TotalAmount"/>
    public int? TotalAmount { get; set; }

    /// <inheritdoc cref="Operation.OverDeliveryAmount"/>
    public int? OverDeliveryAmount { get; set; }

    /// <inheritdoc cref="Operation.UnderDeliveryAmount"/>
    public int? UnderDeliveryAmount { get; set; }

    /// <inheritdoc cref="Operation.TargetCycleTime"/>
    public double? TargetCycleTime { get; set; }

    /// <inheritdoc cref="Operation.TargetStock"/>
    public string TargetStock { get; set; }
}
