// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.TestTools;
using Moryx.Model.Repositories;
using Moryx.Resources.Management.Model;
using NUnit.Framework;

namespace Moryx.Resources.Management.Tests;

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
