// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    [DataContract]
    public class WatchImporterConfig : ProductImporterConfig
    {
        public override string PluginName => nameof(WatchImporter);
    }
}
