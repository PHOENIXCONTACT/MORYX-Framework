// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Moryx.Factory.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Factory
{
    /// <summary>
    /// Transport path in a factory
    /// </summary>
    [Display(Name = nameof(Strings.TransportPath_DisplayName), Description = nameof(Strings.TransportPath_Description), ResourceType = typeof(Strings))]
    public class TransportPath : Resource, ITransportPath
    {
        [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Source)]
        public ILocation Origin { get; set; }

        [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Target)]
        public ILocation Destination { get; set; }

        [DataMember, EntrySerialize]
        [Display(Name = nameof(Strings.TransportPath_WayPoints), ResourceType = typeof(Strings))]
        public List<Position> WayPoints { get; set; } = new List<Position>();
    }
}
