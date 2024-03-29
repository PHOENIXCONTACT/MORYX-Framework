// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    [DataContract]
    public class FileImporterConfig : ProductImporterConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        public override string PluginName => nameof(FileImporter);
    }
}
