// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Products.Model;
using Moryx.Model.Repositories;
using NUnit.Framework;
using Moryx.AbstractionLayer.TestTools;

namespace Moryx.Products.IntegrationTests
{
    [TestFixture]
    public class ProductStorageSqliteTests : ProductStorageTests
    {
        protected override UnitOfWorkFactory<ProductsContext> BuildUnitOfWorkFactory()
        {
            var uowFactory = InMemoryUnitOfWorkFactoryBuilder
                .Sqlite<ProductsContext>();
            uowFactory.EnsureDbIsCreated();

            return uowFactory;
        }
    }
}
