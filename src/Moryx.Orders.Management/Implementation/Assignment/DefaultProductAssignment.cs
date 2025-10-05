// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IProductAssignment), Name = nameof(DefaultProductAssignment))]
    internal class DefaultProductAssignment : ProductAssignmentBase<ProductAssignmentConfig>
    {
        /// <inheritdoc />
        public override Task<IProductType> SelectProduct(Operation operation, IOperationLogger operationLogger)
        {
            var productIdentity = (ProductIdentity)operation.Product.Identity;
            var product = ProductManagement.LoadType(productIdentity);

            return Task.FromResult(product);
        }
    }
}
