// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Serialization;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Configuration of this module
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// If database is empty, this resource will be created by default.
        /// </summary>
        [DataMember, Description("Configuration of possible resource initializer")]
        [PluginConfigs(typeof(IResourceInitializer), true)]
        public List<ResourceInitializerConfig> Initializers { get; set; }
    }
}
