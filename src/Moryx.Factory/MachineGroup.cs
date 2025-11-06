// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Moryx.Factory.Properties;

namespace Moryx.Factory
{
    /// <summary>
    /// Class for all machine groups in manufacturing factory
    /// </summary>
    [Display(Name = nameof(Strings.MachineGroup_DisplayName), ResourceType = typeof(Strings))]
    public class MachineGroup : Resource, IMachineGroup
    {
        [DataMember, EntrySerialize, DefaultValue("settings")]
        public string DefaultIcon { get; set; }
    }
}
