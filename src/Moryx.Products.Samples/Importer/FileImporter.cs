// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    [ExpectedConfig(typeof(FileImporterConfig))]
    [ProductImporter(nameof(FileImporter))]
    public class FileImporter : ProductImporterBase<FileImporterConfig, FileImportParameters>
    {
        /// <summary>
        /// Method to generate an instance of the parameter array
        /// </summary>
        protected override object GenerateParameters()
        {
            return new FileImportParameters { FileExtension = ".mjb" };
        }

        /// <summary>
        /// Import a product using given parameters
        /// </summary>
        protected override Task<ProductImporterResult> ImportAsync(ProductImportContext context, FileImportParameters parameters,
            CancellationToken cancellationToken)
        {
            using (var stream = parameters.ReadFile())
            {
                var textReader = new StreamReader(stream);
                var identifier = textReader.ReadLine();
                var revision = short.Parse(textReader.ReadLine() ?? "0");
                var name = textReader.ReadLine();

                return Task.FromResult(new ProductImporterResult
                {
                    ImportedTypes =
                    [
                        new NeedleType
                        {
                            Name = name,
                            Identity = new ProductIdentity(identifier, revision)
                        }
                    ]
                });
            }
        }
    }
}
