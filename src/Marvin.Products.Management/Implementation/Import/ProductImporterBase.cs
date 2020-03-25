// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;
using Marvin.Products.Management.Importers;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Base class for product importers
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    /// <typeparam name="TParameters">Type of the exchanged parameters object</typeparam>
    public abstract class ProductImporterBase<TConfig, TParameters> : IProductImporter
        where TConfig : ProductImporterConfig
        where TParameters : IImportParameters, new()
    {
        /// <summary>
        /// Config of this importer
        /// </summary>
        protected TConfig Config { get; private set; }

        /// <summary>
        /// Name of the importer
        /// </summary>
        public string Name => Config.PluginName;
        
        /// <summary>
        /// Get the parameters of this importer
        /// </summary>
        public IImportParameters Parameters { get; private set; }

        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        public virtual void Initialize(ProductImporterConfig config)
        {
            Config = (TConfig) config;

            Parameters = GenerateParameters();
        }

        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Stop internal execution of active and/or periodic functionality.
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Generate a new instance of <see cref="IImportParameters"/>
        /// </summary>
        protected virtual IImportParameters GenerateParameters()
        {
            return new TParameters();
        }

        /// <summary>
        /// Update parameters based on partial input
        /// </summary>
        IImportParameters IProductImporter.Update(IImportParameters currentParameters)
        {
            return Update((TParameters) currentParameters);
        }

        /// <summary>
        /// Update the parameters object based on the current values
        /// </summary>
        protected virtual TParameters Update(TParameters currentParameters)
        {
            return currentParameters;
        }

        /// <summary>
        /// Import products using given parameters
        /// </summary>
        IProductType[] IProductImporter.Import(IImportParameters parameters)
        {
            var products = Import((TParameters) parameters);
            Parameters = GenerateParameters();
            return products;
        }

        /// <summary>
        /// Import products using typed parameters
        /// </summary>
        protected abstract IProductType[] Import(TParameters parameters);
    }
}
