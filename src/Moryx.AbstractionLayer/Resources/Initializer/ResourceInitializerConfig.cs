// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Configuration base for <see cref="IResourceInitializer"/>
    /// </summary>
    [DataContract]
    public class ResourceInitializerConfig : UpdatableEntry, IPluginConfig
    {
        /// <inheritdoc />
        [DataMember, Description("Name of the resource initializer")]
        [PluginNameSelector(typeof(IResourceInitializer))]
        [DisplayName("Plugin Name")]
        public virtual string PluginName { get; set; }

        /// <summary>
        /// Overrides <see cref="object.ToString"/> for the plugin name
        /// </summary>
        public override string ToString()
        {
            return PluginName;
        }
    }
}
