// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Material.Endpoints.Model;

[DataContract]
public enum StateClassificationModel
{
    [EnumMember]
    [Display(Name = "Uninitialized", Description = "Container state has not yet been initialized by the lifecycle state machine.")]
    Uninitialized = 0,

    [EnumMember]
    [Display(Name = "Requested", Description = "Material has been requested but not yet announced or registered.")]
    Requested = 1,

    [EnumMember]
    [Display(Name = "Inbound", Description = "Material has been announced as inbound.")]
    Inbound = 2,

    [EnumMember]
    [Display(Name = "Available", Description = "Container is registered and available for use.")]
    Available = 3,

    [EnumMember]
    [Display(Name = "Outbound", Description = "Container has an active pre-advice for departure.")]
    Outbound = 4,

    [EnumMember]
    [Display(Name = "Deregistered", Description = "Container was deregistered from the system after a clean transition.")]
    Deregistered = 5
}
