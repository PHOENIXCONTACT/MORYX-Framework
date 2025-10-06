// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Factory
{
    /// <summary>
    /// Class for all machine groups in manufacturing factory
    /// </summary>
    public class MachineGroup : Resource, IMachineGroup
    {
        [DataMember, EntrySerialize, DefaultValue("settings")]
        public string DefaultIcon { get; set; }
    }
}
