// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints.Models;

[DataContract]
public class OperationUpdateModel
{
    /// <inheritdoc cref="Operation.SortOrder"/>
    [DataMember]
    public int? SortIndex { get; set; }

    /// <inheritdoc cref="Operation.PlannedStart"/>
    [DataMember]
    public DateTime? PlannedStart { get; set; }

    /// <inheritdoc cref="Operation.PlannedEnd"/>
    [DataMember]
    public DateTime? PlannedEnd { get; set; }

    /// <inheritdoc cref="Operation.TotalAmount"/>
    [DataMember]
    public int? TotalAmount { get; set; }

    /// <inheritdoc cref="Operation.OverDeliveryAmount"/>
    [DataMember]
    public int? OverDeliveryAmount { get; set; }

    /// <inheritdoc cref="Operation.UnderDeliveryAmount"/>
    [DataMember]
    public int? UnderDeliveryAmount { get; set; }

    /// <inheritdoc cref="Operation.TargetCycleTime"/>
    [DataMember]
    public double? TargetCycleTime { get; set; }

    /// <inheritdoc cref="Operation.TargetStock"/>
    [DataMember]
    public string TargetStock { get; set; }
}
