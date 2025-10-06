// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.Factory
{
    /// <summary>
    /// Transport path in a factory
    /// </summary>
    public class TransportPath : Resource, ITransportPath
    {
        [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Source)]
        public ILocation Origin { get; set; }

        [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Target)]
        public ILocation Destination { get; set; }

        [DataMember, EntrySerialize]
        public List<Position> WayPoints { get; set; } = new List<Position>();
    }
}
