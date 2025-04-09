// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Products.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Configuration for <see cref="GenericInstanceStrategy"/>
    /// </summary>
    [DataContract]
    public class GenericInstanceConfiguration : ProductInstanceConfiguration, IGenericMapperConfiguration
    {
        public override string PluginName
        {
            get { return nameof(GenericInstanceStrategy); }
            set { }
        }

        /// <inheritdoc />
        [DataMember, DefaultValue(nameof(IGenericColumns.Text8)), AvailableColumns(typeof(string))]
        [Description("Column that should be used to store all non-configured properties as JSON")]
        public string JsonColumn { get; set; }

        /// <inheritdoc />
        [DataMember, Description]
        public List<PropertyMapperConfig> PropertyConfigs { get; set; }
    }
}
