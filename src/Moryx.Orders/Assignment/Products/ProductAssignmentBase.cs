// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Logging;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Base class for assigning a product. Implements <see cref="IProductAssignment"/>
    /// </summary>
    /// <typeparam name="TConfig">Special config type of the implementation</typeparam>
    public abstract class ProductAssignmentBase<TConfig> : IProductAssignment
        where TConfig : ProductAssignmentConfig
    {
        #region Dependencies

        /// <summary>
        /// ProductManagement used to provide the current recipe of a product.
        /// The recipe will be assigned to the operation
        /// </summary>
        public IProductManagement ProductManagement { get; set; }

        /// <summary>
        /// Logger for the specific implementation of <see cref="IProductAssignment"/>
        /// </summary>
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <summary>
        /// Typed config for this component
        /// </summary>
        protected TConfig Config { get; private set; }

        /// <inheritdoc cref="IProductAssignment"/>
        public virtual Task InitializeAsync(ProductAssignmentConfig config)
        {
            Config = (TConfig)config;
            Logger = Logger.GetChild(Config.PluginName, GetType());
            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IProductAssignment"/>
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
        /// Will be called while creating an operation to load the product for the new operation
        /// The origin of the product MUST be the <see cref="ProductManagement"/>
        /// </summary>
        public abstract Task<ProductType> SelectProductAsync(Operation operation, IOperationLogger operationLogger,
            CancellationToken cancellationToken = default);
    }
}
