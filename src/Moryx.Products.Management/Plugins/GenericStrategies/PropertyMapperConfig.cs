// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Configuration how a single property should be stored
    /// </summary>
    [DataContract]
    public class PropertyMapperConfig : IPluginConfig
    {
        /// <summary>
        /// Name of the property on the product
        /// </summary>
        [DataMember]
        public string PropertyName { get; set; }

        /// <summary>
        /// Name of plugin responsible for the property
        /// </summary>
        [DataMember, PluginNameSelector(typeof(IPropertyMapper))]
        public virtual string PluginName { get; set; }

        /// <summary>
        /// Column where the value shall be stored
        /// </summary>
        [DataMember, AvailableColumns]
        public string Column { get; set; }

        public override string ToString()
        {
            return $"{PropertyName}=>{Column}({PluginName})";
        }
    }
}
