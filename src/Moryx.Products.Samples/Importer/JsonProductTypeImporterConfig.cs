// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples;

/// <summary>
/// Importer config for testing JsonProductType
/// </summary>
[DataContract]
public class JsonProductTypeImporterConfig : ProductImporterConfig
{
    /// <summary>
    /// Name of the component represented by this entry
    /// </summary>
    public override string PluginName => nameof(JsonProductTypeImporter);
}
