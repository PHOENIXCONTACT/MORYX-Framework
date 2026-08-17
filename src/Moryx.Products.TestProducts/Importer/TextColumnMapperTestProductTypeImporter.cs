// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.TestProducts;

public class TextColumnMapperTestProductTypeImporter : ProductImporterBase<TextColumnMapperTestProductTypeImporterConfig, TextColumnMapperTestProductTypeParameters>
{
    protected override Task<ProductImporterResult> ImportAsync(ProductImportContext context, TextColumnMapperTestProductTypeParameters parameters, CancellationToken cancellationToken)
    {
        var product = new TextColumnMapperTestProductType
        {
            Name = parameters.Name,
            Identity = new ProductIdentity(parameters.Identifier, parameters.Revision),

            ComplexData1 = new ComplexData
            {
                PropertyName = "First",
                Number = 123,
                Weight = 1.23f
            },
        };

        return Task.FromResult(new ProductImporterResult
        {
            ImportedTypes = [product]
        });
    }
}

public class TextColumnMapperTestProductTypeParameters
    : PrototypeParameters
{
}
