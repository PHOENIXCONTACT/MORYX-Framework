// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Products;
using Moryx.Products.Samples.Importer;

namespace Moryx.Products.Samples;

[DataContract]
public class TextColumnMapperTestProductTypeImporterConfig
    : ProductImporterConfig
{
    public override string PluginName =>
        nameof(TextColumnMapperTestProductTypeImporter);
}
