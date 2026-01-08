// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Factory.Properties;

using Moryx.Serialization;

namespace Moryx.Factory
{
    [ResourceRegistration]
    [Display(Name = nameof(Strings.ManufacturingFactory_DisplayName), ResourceType = typeof(Strings))]
    public class ManufacturingFactory : Resource, IManufacturingFactory
    {
        [DataMember, EntrySerialize, DefaultValue("https://www.phoenixcontact.com/common/assets/images/signal-chain.svg")]
        [Display(Name = nameof(Strings.ManufacturingFactory_BackgroundUrl), Description = nameof(Strings.ManufacturingFactory_BackgroundUrl_Description), ResourceType = typeof(Strings))]
        public string BackgroundUrl { get; set; }
    }
}
