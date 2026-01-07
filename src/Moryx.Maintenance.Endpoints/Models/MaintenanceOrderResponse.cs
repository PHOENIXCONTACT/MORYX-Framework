// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.VisualInstructions;
using Moryx.Maintenance.Properties;
using Moryx.Serialization;

namespace Moryx.Maintenance.Endpoints.Models;

/// <summary>
/// UI specific Dto for a maintenance order
/// </summary>
[DataContract, EntrySerialize]
[Display(ResourceType = typeof(Strings), Name = nameof(Strings.MaintenanceOrderResponse_MaintenanceForm))]
public class MaintenanceOrderResponse
{
    /// <summary>
    /// The resource this maintenance order belongs to
    /// </summary>
    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.MaintenanceOrderResponse_Resource),
        Description = nameof(Strings.MaintenanceOrderResponse_Resource_Description)
        )]
    [MaintainableResources, Required, DataMember]
    public string Resource { get; set; } = string.Empty;

    /// <summary>
    /// Description of the maintenance order
    /// </summary>
    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.MaintenanceOrderResponse_Description)
        )]
    [DataMember]
    public string? Description { get; set; }

    /// <summary>
    /// Interval of the maintenance order
    /// </summary>
    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.MaintenanceOrderResponse_Interval),
        Description = nameof(Strings.MaintenanceOrderResponse_Interval_Description)
        )]
    [EntryPrototypes(typeof(IntervalBase)), Required, DataMember]
    public IntervalBase Interval { get; set; } = new Days();

    /// <summary>
    /// Visual instructions for the maintenance order
    /// </summary>
    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.MaintenanceOrderResponse_Instructions)
        )]
    [DataMember]
    public VisualInstruction[] Instructions { get; set; } = [];

    /// <summary>
    /// Whether the maintenance order should block the resource
    /// </summary>
    [Display(
        ResourceType = typeof(Strings),
        Name = nameof(Strings.MaintenanceOrderResponse_Block),
        Description = nameof(Strings.MaintenanceOrderResponse_Block_Description)
        )]
    [EntrySerialize, DataMember]
    public bool Block { get; set; }

    /// <summary>
    /// Whether the maintenance order is active
    /// </summary>

    [Display(
       ResourceType = typeof(Strings),
       Name = nameof(Strings.MaintenanceOrderResponse_IsActive),
       Description = nameof(Strings.MaintenanceOrderResponse_IsActive_Description)
       )]
    [DefaultValue(true), DataMember]
    public bool IsActive { get; set; }

    /// <summary>
    /// Creation date of the maintenance order
    /// </summary>
    [EntrySerialize(EntrySerializeMode.Never), ReadOnly(true), DataMember]
    public DateTime Created { get; set; }
}
