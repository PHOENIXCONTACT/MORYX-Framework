// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Config of the document loader to get the needed document information
    /// </summary>
    public class DocumentLoaderConfig : IPluginConfig
    {
        /// <inheritdoc />
        [DataMember]
        [PluginNameSelector(typeof(IDocumentLoader))]
        public virtual string PluginName { get; set; }
    }
}
