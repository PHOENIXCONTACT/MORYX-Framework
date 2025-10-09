// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Factory.Localizations;

using Moryx.Serialization;

namespace Moryx.Factory
{
    [ResourceRegistration]
    [Display(Name = nameof(Strings.MANUFACTURING_FACTORY), ResourceType = typeof(Localizations.Strings))]
    public class ManufacturingFactory : Resource, IManufacturingFactory
    {
        [DataMember, EntrySerialize, DefaultValue("https://www.phoenixcontact.com/common/assets/images/signal-chain.svg")]
        [Display(Name = nameof(Strings.BACKGROUND_URL), Description = nameof(Strings.BACKGROUND_URL_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
        public string BackgroundUrl { get; set; }
    }
}
