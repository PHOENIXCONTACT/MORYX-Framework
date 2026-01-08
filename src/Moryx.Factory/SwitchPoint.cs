// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Moryx.Factory.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Factory;

/// <summary>
/// Point where the direction changes in a transport path.
/// </summary>
[Display(Name = nameof(Strings.SwitchPoint_DisplayName), Description = nameof(Strings.SwitchPoint_Description), ResourceType = typeof(Strings))]
public class SwitchPoint : Resource, ILocation
{

    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.MachineLocation_PositionX), ResourceType = typeof(Strings))]
    public double PositionX { get; set; }

    [DataMember, EntrySerialize]
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

    public IEnumerable<ITransportPath> Origins { get; set; }

    public IEnumerable<ITransportPath> Destinations { get; set; }

    public string Image { get; set; }
}