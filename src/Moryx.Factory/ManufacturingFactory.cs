// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;

using Moryx.Serialization;

namespace Moryx.Factory
{
    public class ManufacturingFactory : Resource, IManufacturingFactory
    {
        [DataMember, EntrySerialize, DefaultValue("assets/Fabrik_Hintergrund.png"), Description("URL of the background picture of the Factory Monitor")]
        public string BackgroundUrl { get; set; }
    }
}
