// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moryx.AbstractionLayer.Products;
using Moryx.Products.Management.Importers;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Base class for product importers
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    /// <typeparam name="TParameters">Type of the exchanged parameters object</typeparam>
    public abstract class ProductImporterBase<TConfig, TParameters> : IProductImporter
        where TConfig : ProductImporterConfig
        where TParameters : new()
    {
        /// <summary>
        /// Config of this importer
        /// </summary>
        protected TConfig Config { get; private set; }
        
        /// <inheritdoc />
        public string Name => Config.PluginName;

        /// <inheritdoc />
        public virtual bool LongRunning => false;

        /// <summary>
        /// Parameters of this importer
        /// </summary>
        public object Parameters { get; private set; }

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
        /// Generate a new instance of parameters
        /// </summary>
        protected virtual object GenerateParameters()
        {
            return new TParameters();
        }

        /// <summary>
        /// Update parameters based on partial input
        /// </summary>
        object IProductImporter.Update(object currentParameters)
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
        Task<ProductImporterResult> IProductImporter.Import(ProductImportContext context, object parameters)
        {
            var result = Import(context, (TParameters) parameters);
            Parameters = GenerateParameters();
            return result;
        }

        /// <summary>
        /// Import products using typed parameters
        /// </summary>
        protected abstract Task<ProductImporterResult> Import(ProductImportContext context, TParameters parameters);
    }
}
