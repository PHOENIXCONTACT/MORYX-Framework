// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
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
        /// List of configured resource initializers
        /// </summary>
        [DataMember, Description("List of configured resource initializers")]
        [PluginConfigs(typeof(IResourceInitializer), true)]
        public List<ResourceInitializerConfig> Initializers { get; set; }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            Initializers = new List<ResourceInitializerConfig>();
        }
    }
}
