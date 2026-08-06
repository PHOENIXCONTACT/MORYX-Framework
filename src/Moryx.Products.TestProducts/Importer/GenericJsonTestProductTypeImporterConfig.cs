// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.TestProducts;

[DataContract]
public class GenericJsonTestProductTypeImporterConfig : ProductImporterConfig
{
    /// <summary>
    /// Name of the component represented by this entry
    /// </summary>
    public override string PluginName => nameof(GenericJsonTestProductTypeImporter);
}
