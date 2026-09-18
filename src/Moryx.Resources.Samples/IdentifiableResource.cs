// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.Resources.Samples;

[ResourceAvailableAs(typeof(IIdentifiableObject))]
public class IdentifiableResource : Resource, IIdentifiableObject
{
    [EntrySerialize, DataMember]
    public IIdentity Identity {  get; set; }
}
