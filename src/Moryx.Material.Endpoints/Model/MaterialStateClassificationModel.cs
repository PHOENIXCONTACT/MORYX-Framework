// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Endpoints.Model;

[DataContract]
public enum MaterialStateClassificationModel
{
    [EnumMember]
    Uninitialized = 0,

    [EnumMember]
    Requested = 1,

    [EnumMember]
    Inbound = 2,

    [EnumMember]
    Available = 3,

    [EnumMember]
    Outbound = 4,

    [EnumMember]
    Deregistered = 5
}
