// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.TestTools;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.Model.Repositories;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ProcessStorageSqliteTests : ProcessStorageTests
    {
        protected override UnitOfWorkFactory<ProcessContext> BuildUnitOfWorkFactory()
        {
            return InMemoryUnitOfWorkFactoryBuilder
                .Sqlite<ProcessContext>()
                .EnsureDbIsCreated();
        }
    }
}

