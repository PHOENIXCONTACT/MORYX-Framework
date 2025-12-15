// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.TestTools.Test.Model;
using NUnit.Framework;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.TestTools;
using System;
using System.Collections.Generic;
using Moryx.Runtime.Endpoints.Databases;
using Moryx.Runtime.Endpoints.Databases.Models;

namespace Moryx.Runtime.Endpoints.IntegrationTests.Databases.Controller
{
    internal class ModelSetupTests
    {
        private IDbContextManager? _dbContextManager;
        private DatabaseController? _databaseController;
        private readonly List<Exception> _exceptions = [];

        [SetUp]
        public void Setup()
        {
            _dbContextManager = InMemoryUnitOfWorkFactoryBuilder
                .SqliteDbContextManager();

            _dbContextManager
                .Factory<TestModelContext>()
                .EnsureDbIsCreated();

            _databaseController = new DatabaseController(_dbContextManager);

            _exceptions.Clear();
        }

        [Test]
        public async Task ExecuteSetupDoesNotThrowDisposedObjectException()
        {
            // Add unobserved task exceptions to a list, to be checked later.
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                _exceptions.Add(e.Exception);
            };

            var result = await _databaseController!.ExecuteSetup(typeof(TestModelContext).FullName, new()
            {
                Config = new()
                {
                    ConfiguratorTypename = "1",
                    Entries = new() { { "ConnectionString", "DataSource=:memory:" } }
                },
                Setup = new SetupModel { Fullname = typeof(DisposedObjectSetup).FullName }
            });

            // ExecuteSetup lead to unobserved task exceptions. We give them
            // some time to be thrown.
            Task.Delay(100).Wait();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.That(result?.Value?.Success, Is.True);
            Assert.That(0, Is.EqualTo(_exceptions.Count));
        }
    }
}
