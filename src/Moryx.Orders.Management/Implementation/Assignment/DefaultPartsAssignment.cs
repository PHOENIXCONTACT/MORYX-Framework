// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment
{
    /// <summary>
    /// Class for assigning product parts from the product type. Implements <see cref="IPartsAssignment"/>
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IPartsAssignment), Name = nameof(DefaultPartsAssignment))]
    internal class DefaultPartsAssignment : IPartsAssignment
    {
        /// <summary>
        /// Typed config for this component
        /// </summary>
        protected PartsAssignmentConfig Config { get; private set; }

        /// <inheritdoc/>
        public Task InitializeAsync(PartsAssignmentConfig config, CancellationToken cancellationToken = default)
        {
            Config = config;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Will be called while creating an operation to load the part list for
        /// the new operation from the <see cref="ProductType"/> itself.
        /// </summary>
        public Task<IReadOnlyList<ProductPart>> LoadPartsAsync(Operation operation, IOperationLogger operationLogger,
            CancellationToken cancellationToken)
        {
            if (operation.Product is null)
            {
                return Task.FromResult((IReadOnlyList<ProductPart>)Array.Empty<ProductPart>());
            }

            var countedParts = new Dictionary<ProductType, uint>();
            IterateProductParts(operation.Product, countedParts);
            var result = countedParts.Select(pt => new ProductPart()
            {
                Id = pt.Key.Id,
                Identity = pt.Key.Identity as ProductIdentity ?? new ProductIdentity(pt.Key.Identity.Identifier, 0),
                Name = pt.Key.Name,
                State = pt.Key.State,
                Classification = PartClassification.Unknown,
                StagingIndicator = StagingIndicator.NotRelevant, // Unknown here
                Quantity = pt.Value
            }).ToArray();

            return Task.FromResult((IReadOnlyList<ProductPart>)result);
        }

        private static void IterateProductParts(ProductType product, Dictionary<ProductType, uint> countedParts)
        {
            var productType = product.GetType();
            IteratePartLinks(product, countedParts, productType);
            IteratePartLinkCollections(product, countedParts, productType);
        }

        private static void IteratePartLinks(ProductType product, Dictionary<ProductType, uint> countedParts, Type productType)
        {
            var properties = productType.GetProperties()
                            .Where(p => p.PropertyType.IsAssignableTo(typeof(ProductPartLink)));
            foreach (var property in properties)
            {
                if (property.GetValue(product) is not ProductPartLink propertyValue)
                {
                    continue;
                }

                var partProductType = propertyValue.Product;
                if (countedParts.TryGetValue(partProductType, out var _))
                {
                    countedParts[partProductType]++;
                }
                else
                {
                    countedParts.Add(partProductType, 1);
                }

                IterateProductParts(partProductType, countedParts);
            }
        }

        private static void IteratePartLinkCollections(ProductType product, Dictionary<ProductType, uint> countedParts, Type productType)
        {
            var properties = productType.GetProperties().Where(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                p.PropertyType.GetGenericArguments()[0].IsAssignableTo(typeof(ProductPartLink))
            );

            foreach (var property in properties)
            {
                if (property.GetValue(product) is not IEnumerable<ProductPartLink> propertyValue || !propertyValue.Any())
                {
                    continue;
                }

                foreach (var partProductType in propertyValue.Select(link => link.Product))
                {
                    if (countedParts.TryGetValue(partProductType, out var _))
                    {
                        countedParts[partProductType]++;
                    }
                    else
                    {
                        countedParts.Add(partProductType, 1);
                    }

                    IterateProductParts(partProductType, countedParts);
                }
            }
        }
    }
}
