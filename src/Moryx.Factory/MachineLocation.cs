// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.Serialization;
using Moryx.Factory.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Factory;

/// <summary>
/// Class for MachineLocation in the factory
/// </summary>
[ResourceRegistration]
// TODO: ADD value
[Display(Name = nameof(Strings.MachineLocation_DisplayName), ResourceType = typeof(Strings))]
public class MachineLocation : Resource, IMachineLocation
{
    public IResource Machine => Children.OfType<ICell>().FirstOrDefault();

    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.MachineLocation_SpecificIcon), ResourceType = typeof(Strings))]
    public string SpecificIcon { get; set; }

    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.MachineLocation_Image), ResourceType = typeof(Strings))]
    public string Image { get; set; }

    /// <summary>
    /// X position of the location
    /// </summary>
    [DataMember, EntrySerialize, DefaultValue(0.5)]
    [Display(Name = nameof(Strings.MachineLocation_PositionX), ResourceType = typeof(Strings))]
    public double PositionX { get; set; }

    /// <summary>
    /// Y position of the location
    /// </summary>
    [DataMember, EntrySerialize, DefaultValue(0.5)]
    [Display(Name = nameof(Strings.MachineLocation_PositionY), ResourceType = typeof(Strings))]
    public double PositionY { get; set; }

    public Position Position
    {
        get => new() { PositionX = PositionX, PositionY = PositionY };
        set
        {
            PositionX = value.PositionX;
            PositionY = value.PositionY;
        }
    }

    [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Source)]
    public IReferences<ITransportPath> Origins { get; set; }

    [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Target)]
    public IReferences<ITransportPath> Destinations { get; set; }

    IEnumerable<ITransportPath> ILocation.Origins => Origins;
    IEnumerable<ITransportPath> ILocation.Destinations => Destinations;
}