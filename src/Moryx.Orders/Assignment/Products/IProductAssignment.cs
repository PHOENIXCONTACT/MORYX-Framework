// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Modules;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Will be used to assign a product to the operation based on existing data.
    /// </summary>
    public interface IProductAssignment : IConfiguredPlugin<ProductAssignmentConfig>
    {
        /// <summary>
        /// Will be called while creating an operation to load the product for the new operation
        /// The origin of the product MUST be the <see cref="IProductManagement"/>
        /// </summary>
        Task<IProductType> SelectProduct(Operation operation, IOperationLogger operationLogger);
    }
}