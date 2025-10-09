// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Moryx.Factory.Localizations;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Factory
{
    /// <summary>
    /// Point where the direction changes in a transport path.
    /// </summary>
    [Display(Name = nameof(Strings.SWITCH_POINT), Description = nameof(Strings.SWITCH_POINT_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public class SwitchPoint : Resource, ILocation
    {

        [DataMember, EntrySerialize]
        [Display(Name = nameof(Strings.POSITION_X), ResourceType = typeof(Localizations.Strings))]
        public double PositionX { get; set; }

        [DataMember, EntrySerialize]
        [Display(Name = nameof(Strings.POSITION_Y), ResourceType = typeof(Localizations.Strings))]
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
}
