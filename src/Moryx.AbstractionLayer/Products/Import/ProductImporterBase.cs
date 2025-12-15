// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products
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

        /// <inheritdoc />
        public object Parameters { get; private set; }

        /// <inheritdoc />
        public virtual Task InitializeAsync(ProductImporterConfig config, CancellationToken cancellationToken = default)
        {
            Config = (TConfig)config;

            Parameters = GenerateParameters();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
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
            return Update((TParameters)currentParameters);
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
        Task<ProductImporterResult> IProductImporter.ImportAsync(ProductImportContext context, object parameters, CancellationToken cancellationToken)
        {
            var result = ImportAsync(context, (TParameters)parameters, cancellationToken);
            Parameters = GenerateParameters();
            return result;
        }

        /// <summary>
        /// Import products using typed parameters
        /// </summary>
        protected abstract Task<ProductImporterResult> ImportAsync(ProductImportContext context, TParameters parameters, CancellationToken cancellationToken);
    }
}
