// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Base config for <see cref="IProductAssignment"/> implementations
    /// </summary>
    [DataContract]
    public class ProductAssignmentConfig : IPluginConfig
    {
        /// <inheritdoc cref="IPluginConfig"/>
        [DataMember]
        [PluginNameSelector(typeof(IProductAssignment))]
        public virtual string PluginName { get; set; }
    }
}