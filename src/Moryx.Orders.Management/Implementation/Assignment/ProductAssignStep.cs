// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Orders.Assignment;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationAssignStep), Name = nameof(ProductAssignStep))]
    internal class ProductAssignStep : IOperationAssignStep
    {
        public IProductManagement ProductManagement { get; set; }

        public IProductAssignment ProductAssignment { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public async Task<bool> AssignStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            var product = await ProductAssignment.SelectProduct(operationData.Operation, operationLogger);
            if (product == null)
            {
                operationLogger.Log(LogLevel.Error, Strings.ProductAssignStep_Selection_Failed);
                return false;
            }

            operationData.AssignProduct(product);
            return true;
        }

        public Task<bool> RestoreStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            if (operationData.Product == null)
                return Task.FromResult(false);

            var productId = operationData.Product.Id;

            IProductType product = null;
            if (productId != 0)
                product = ProductManagement.LoadType(operationData.Product.Id);

            if (product == null)
            {
                operationLogger.Log(LogLevel.Warning, Strings.ProductAssignStep_Selection_Failed);
                return Task.FromResult(false);
            }

            operationData.AssignProduct(product);
            return Task.FromResult(true);
        }
    }
}
