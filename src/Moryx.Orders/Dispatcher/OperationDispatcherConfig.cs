// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Orders.Dispatcher
{
    /// <summary>
    /// The operation dispatcher config
    /// </summary>
    [DataContract]
    public class OperationDispatcherConfig : IPluginConfig
    {
        /// <inheritdoc />
        [DataMember]
        [PluginNameSelector(typeof(IOperationDispatcher))]
        public virtual string PluginName { get; set; }
    }
}