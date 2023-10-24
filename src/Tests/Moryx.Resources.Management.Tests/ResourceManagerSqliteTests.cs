// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Resources.Model;
using Moryx.Model.Repositories;
using NUnit.Framework;
using Moryx.AbstractionLayer.TestTools;

namespace Moryx.Resources.Management.Tests
{
    [TestFixture]
    public class ResourceManagerSqliteTests : ResourceManagerTests
    {
        protected override UnitOfWorkFactory<ResourcesContext> BuildUnitOfWorkFactory()
        {
            var uowFactory = InMemoryUnitOfWorkFactoryBuilder
                .Sqlite<ResourcesContext>();
            uowFactory.EnsureDbIsCreated();

            return uowFactory;
        }
    }
}
