// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Modules;
using Moryx.Products.Management;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples;

[ExpectedConfig(typeof(WatchImporterConfig))]
[ProductImporter(nameof(WatchImporter))]
public class WatchImporter : ProductImporterBase<WatchImporterConfig, SpecializedWatchImportParameters>
{
    public IProductStorage Storage { get; set; }

    protected override async Task<ProductImporterResult> ImportAsync(ProductImportContext context, SpecializedWatchImportParameters parameters,
        CancellationToken cancellationToken)
    {
        var product = new WatchType
        {
            Name = parameters.Name,
            Identity = new ProductIdentity(parameters.Identifier, parameters.Revision),
            WatchFace = new ProductPartLink<WatchFaceTypeBase>
            {
                Product = (WatchFaceType)await Storage.LoadTypeAsync(new ProductIdentity(parameters.WatchfaceIdentifier, ProductIdentity.LatestRevision), cancellationToken)
            },
            Needles =
            [
                new NeedlePartLink
                {
                    Role = NeedleRole.Minutes,
                    Product = (NeedleType)await Storage.LoadTypeAsync(new ProductIdentity(parameters.MinuteNeedleIdentifier,
                        ProductIdentity.LatestRevision), cancellationToken)
                }
            ]
        };

        return new ProductImporterResult
        {
            ImportedTypes = [product]
        };
    }
}

public class SpecializedWatchImportParameters : PrototypeParameters
{
    [Required]
    public string WatchfaceIdentifier { get; set; }

    [Required]
    public string MinuteNeedleIdentifier { get; set; }
}