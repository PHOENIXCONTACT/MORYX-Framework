// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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
    public class PartsAssignmentConfig : IPluginConfig
    {
        /// <inheritdoc cref="IPluginConfig"/>
        [DataMember]
        [PluginNameSelector(typeof(IPartsAssignment))]
        public virtual string PluginName { get; set; }
    }
}