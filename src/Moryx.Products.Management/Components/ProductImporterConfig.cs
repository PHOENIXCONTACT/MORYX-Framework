// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Marvin.Modules;

namespace Marvin.Products.Management.Importers
{
    /// <summary>
    /// Config for product importers
    /// </summary>
    [DataContract]
    public abstract class ProductImporterConfig : IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        public abstract string PluginName { get; }
    }
}
