// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.Maintenance.Attributes;
using Moryx.Maintenance.IntervalTypes;
using Moryx.Maintenance.Localizations;
using Moryx.Serialization;

namespace Moryx.Maintenance.Endpoints.Dtos;

/// <summary>
/// UI specific Dto for a maintenance order
/// </summary>
[DataContract, EntrySerialize]
[Display(ResourceType = typeof(Strings), Name = nameof(Strings.MAINTENANCE_FORM))]
public class MaintenanceOrderResponse
{
    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.RESOURCE),
        Description = nameof(Strings.RESOURCE_DESCRIPTION)
        )]
    [MaintainableResources, Required, DataMember]
    public string Resource { get; set; }

    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.DESCRIPTION)
        )]
    [DataMember]
    public string? Description { get; set; }

    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.INTERVAL),
        Description = nameof(Strings.INTERVAL_DESCRIPTION)
        )]
    [EntryPrototypes(typeof(IntervalBase)), Required, DataMember]
    public IntervalBase Interval { get; set; } = new Days();

    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.INSTRUCTIONS),
        Description = nameof(Strings.INTERVAL_DESCRIPTION)
        )]
    [DataMember]
    public VisualInstruction[] Instructions { get; set; } = [];

    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.BLOCK),
        Description = nameof(Strings.BLOCK_DESCRIPTION)
        )]
    [EntrySerialize, DataMember]
    public bool Block { get; set; }

    [Display(
       ResourceType = typeof(Strings),
       Name = nameof(Strings.IS_ACTIVE),
       Description = nameof(Strings.IS_ACTIVE_DESCRIPTION)
       )]
    [DefaultValue(true), DataMember]
    public bool IsActive { get; set; }

    [EntrySerialize(EntrySerializeMode.Never), ReadOnly(true), DataMember]
    public DateTime Created { get; set; }
}
