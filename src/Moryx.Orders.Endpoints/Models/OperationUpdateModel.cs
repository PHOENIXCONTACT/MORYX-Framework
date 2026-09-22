// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints.Models;

[DataContract]
public class OperationUpdateModel
{
    /// <summary>
    /// <inheritdoc cref="Operation.SortOrder"/>
    /// </summary>
    [DataMember]
    public int? SortIndex { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.PlannedStart"/>
    /// </summary>
    [DataMember]
    public DateTime? PlannedStart { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.PlannedEnd"/>
    /// </summary>
    [DataMember]
    public DateTime? PlannedEnd { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.TotalAmount"/>
    /// </summary>
    [DataMember]
    public int? TotalAmount { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.OverDeliveryAmount"/>
    /// </summary>
    [DataMember]
    public int? OverDeliveryAmount { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.UnderDeliveryAmount"/>
    /// </summary>
    [DataMember]
    public int? UnderDeliveryAmount { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.TargetCycleTime"/>
    /// </summary>
    [DataMember]
    public double? TargetCycleTime { get; set; }

    /// <summary>
    /// <inheritdoc cref="Operation.TargetStock"/>
    /// </summary>
    [DataMember]
    public string TargetStock { get; set; }
}
