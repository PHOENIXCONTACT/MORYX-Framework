// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Modules;

// ReSharper disable AsyncMethodWithoutAwait

namespace Moryx.Products.Samples;

[ExpectedConfig(typeof(GenericJsonTestProductTypeImporterConfig))]
[ProductImporter(nameof(GenericJsonTestProductTypeImporter))]
public class GenericJsonTestProductTypeImporter : ProductImporterBase<GenericJsonTestProductTypeImporterConfig, SpecializedJsonTestProductTypeParameters>
{
    protected override async Task<ProductImporterResult> ImportAsync(ProductImportContext context, SpecializedJsonTestProductTypeParameters parameters,
        CancellationToken cancellationToken)
    {

        var product = new GenericJsonTestProductType()
        {
            Name = parameters.Name,
            Identity = new ProductIdentity(parameters.Identifier, parameters.Revision),

            //Text1 = "T1",
            Text2 = "T2",
            Text3 = "T3",
            Text4 = "T4",
            Text5 = "T5",
            Text6 = "T6",
            Text7 = "T7",
            Text8 = "T8",
            Text9 = "T9",
            Text10 = "T10",

            Integer1 = 1,
            Integer2 = 2,
            Integer3 = 3,
            Integer4 = 4,
            Integer5 = 5,
            Integer6 = 6,
            Integer7 = 7,
            Integer8 = 8,
            Integer9 = 9,
            Integer10 = 10,

            Float1 = 1.23f,
            Float2 = 2.34f,
            Float3 = 3.45f,
            Float4 = 4.56f,
            Float5 = 5.67f,
            Float6 = 6.78f,
            Float7 = 7.89f,
            Float8 = 8.90f,
            Float9 = 9.01f,
            Float10 = 10.11f,

            ComplexData1 = new ComplexData()

            
        };

        var result = new ProductImporterResult
        {
            ImportedTypes = [product]
        };

        return result;
    }
}

public class SpecializedJsonTestProductTypeParameters : PrototypeParameters
{
}
