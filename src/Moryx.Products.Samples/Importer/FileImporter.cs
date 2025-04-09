// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    [ExpectedConfig(typeof(FileImporterConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IProductImporter), Name = nameof(FileImporter))]
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
        protected override Task<ProductImporterResult> Import(ProductImportContext context, FileImportParameters parameters)
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
