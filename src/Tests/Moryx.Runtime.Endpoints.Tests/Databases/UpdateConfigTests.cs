// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Model;
using Moryx.Runtime.Kernel;
using Moryx.TestTools.Test.Model;
using System;
using Moryx.Configuration;
using Moryx.Model.Sqlite;
using Moryx.Runtime.Endpoints.Databases.Services;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Runtime.Endpoints.Tests.Databases
{
    internal class UpdateConfigTests
    {
        private DbContextManager _dbContextManager;
        private DatabaseConfigUpdateService _databaseConfigUpdateService;
        private Type _configuratorType;

        [SetUp]
        public void Setup()
        {
            var configManager = new ConfigManager
            {
                ConfigDirectory = ""
            };

            // Ensure that the assembly containing TestModelContext is loaded
            AppDomainBuilder.LoadAssemblies();

            _dbContextManager = new DbContextManager(configManager, new LoggerFactory());
            _databaseConfigUpdateService = new DatabaseConfigUpdateService(_dbContextManager);
            _configuratorType = typeof(SqliteModelConfigurator);
        }

        [Test(Description = "Leaving Database property empty leads to filling with DbContext type name")]
        public void LeavingDatabaseEmptyDefaultsToDbContextName()
        {
            // Arrange
            var config = new SqliteDatabaseConfig();
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddProviders([new DefaultValueAttributeProvider()]));
            config.UpdateConnectionString();

            // Act
            var result = _databaseConfigUpdateService.UpdateModel(typeof(TestModelContext), config);

            // Assert
            var updatedConfig = GetUpdatedConfig();
            Assert.That(result, Is.EqualTo(typeof(TestModelContext)));
            Assert.That(updatedConfig.ConfiguratorType, Does.Contain(_configuratorType.FullName));
            Assert.That(updatedConfig.DataSource, Is.EqualTo($"./db/{nameof(TestModelContext)}.db"));
            Assert.That(updatedConfig.ConnectionString, Is.EqualTo("Data Source=./db/TestModelContext.db;Mode=ReadWrite;Cache=Default;"));
        }

        [Test]
        public void LeavingConnectionStringEmptyDoesNotFail()
        {
            // Arrange
            var config = new SqliteDatabaseConfig();
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddProviders([new DefaultValueAttributeProvider()]));

            // Act & Assert
            Assert.DoesNotThrow(() => _databaseConfigUpdateService.UpdateModel(typeof(TestModelContext), config));
        }

        private SqliteDatabaseConfig GetUpdatedConfig()
        {
            var configurator = _dbContextManager.GetConfigurator(typeof(TestModelContext));
            return (SqliteDatabaseConfig)configurator.Config;
        }
    }
}
