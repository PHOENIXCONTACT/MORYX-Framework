// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Base config for operation validators <see cref="IOperationValidation"/>
    /// </summary>
    [DataContract]
    public class OperationValidationConfig : IPluginConfig
    {
        /// <inheritdoc cref="IOperationValidation"/>
        [DataMember]
        [PluginNameSelector(typeof(IOperationValidation))]
        public virtual string PluginName { get; set; }
    }
}