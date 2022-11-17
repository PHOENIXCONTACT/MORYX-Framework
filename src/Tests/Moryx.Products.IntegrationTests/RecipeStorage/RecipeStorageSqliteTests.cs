// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.TestTools;
using Moryx.Model.Repositories;
using Moryx.Products.Model;
using NUnit.Framework;

namespace Moryx.Products.IntegrationTests
{
    [TestFixture]
    public class RecipeStorageSqliteTests : RecipeStorageTests
    {

        protected override UnitOfWorkFactory<ProductsContext> BuildUnitOfWorkFactory()
        {
            return InMemoryUnitOfWorkFactoryBuilder
                .Sqlite<ProductsContext>()
                .EnsureDbIsCreated();
        }
    }
}
