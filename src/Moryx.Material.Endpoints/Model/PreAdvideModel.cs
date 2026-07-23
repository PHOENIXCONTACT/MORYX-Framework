// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Endpoints.Model;

[DataContract]
public enum PreAdviceDepartureReasonModel
{
    [EnumMember]
    FinishedGoods = 0,

    [EnumMember]
    UnusedMaterial = 1,

    [EnumMember]
    Transfer = 2,

    [EnumMember]
    Scrap = 3,

    [EnumMember]
    Other = 4
}

[DataContract]
public class PreAdvideModel
{
    [DataMember]
    public long ContainerId { get; set; }

    [DataMember]
    public PreAdviceDepartureReasonModel DepartureReason { get; set; }
}