// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IProductAssignment), Name = nameof(DefaultProductAssignment))]
    internal class DefaultProductAssignment : ProductAssignmentBase<ProductAssignmentConfig>
    {
        /// <inheritdoc />
        public override Task<ProductType> SelectProductAsync(Operation operation, IOperationLogger operationLogger,
            CancellationToken cancellationToken = default)
        {
            var productIdentity = (ProductIdentity)operation.Product.Identity;
            return ProductManagement.LoadTypeAsync(productIdentity, cancellationToken);
        }
    }
}
