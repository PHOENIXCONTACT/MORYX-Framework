// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints.Models;

[DataContract]
public class OperationUpdateModel
{
    [DataMember]
    public int? SortIndex { get; set; }

    [DataMember]
    public DateTime? PlannedStart { get; set; }

    [DataMember]
    public DateTime? PlannedEnd { get; set; }
}
