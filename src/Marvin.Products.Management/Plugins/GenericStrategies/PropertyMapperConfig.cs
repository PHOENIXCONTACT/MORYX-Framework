using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Marvin.Configuration;
using Marvin.Modules;
using Marvin.Serialization;

namespace Marvin.Products.Management
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