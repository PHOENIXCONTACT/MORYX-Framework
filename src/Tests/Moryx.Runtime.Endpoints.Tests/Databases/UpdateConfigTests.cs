// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Model;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Services;
using Moryx.Runtime.Kernel;
using Moryx.TestTools.Test.Model;
using System;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;
using Moryx.Model.Sqlite;
using NUnit.Framework;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Exceptions;

namespace Moryx.Runtime.Endpoints.Tests.Databases
{
    internal class UpdateConfigTests
    {
        private DbContextManager _dbContextManager;
        private DatabaseConfigUpdateService _databaseConfigUpdateService;
        private string _targetModelTypename;
        private Type _configuratorType;

        [SetUp]
        public void Setup()
        {
            _dbContextManager = new DbContextManager(CreateConfigManager(), new LoggerFactory());
            _databaseConfigUpdateService = new DatabaseConfigUpdateService(_dbContextManager);
            _targetModelTypename = typeof(TestModelContext).FullName;
            _configuratorType = typeof(SqliteModelConfigurator);
        }

        [Test(Description = "Leaving Database property empty leads to filling with DbContext type name")]
        public void LeavingDatabaseEmptyDefaultsToDbContextName()
        {
            // Arrange
            var config = SqliteConfigModel()
                .WithDefaults();

            // Act
            var result = _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config);

            // Assert
            var updatedConfig = GetUpdatedConfig();
            Assert.That(result, Is.EqualTo(typeof(TestModelContext)));
            Assert.That(updatedConfig.ConfiguratorTypename, Does.Contain(_configuratorType.FullName));
            Assert.That(updatedConfig.ConnectionSettings.Database, Is.EqualTo(nameof(TestModelContext)));
            Assert.That(updatedConfig.ConnectionSettings.ConnectionString, Is.EqualTo("Data Source=.\\db-filename.db"));
        }

        [Test]
        public void LeavingConnectionStringEmptyFails()
        {
            var config = SqliteConfigModel()
                .WithDefaults()
                .WithConnectionString("");

            BadRequestException exception = Assert.Throws<BadRequestException>(
                () => _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config));

            Assert.That(exception.Message, Is.EqualTo("Requested config values aren't valid"));
        }

        [Test]
        public void InvalidConnectionStringFails()
        {
            var config = SqliteConfigModel()
                .WithDefaults()
                .WithConnectionString("rubbish");

            BadRequestException exception = Assert.Throws<BadRequestException>(
                () => _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config));

            Assert.That(exception.Message, Is.EqualTo("Requested config values aren't valid"));
        }

        [Test]
        public void ValidConnectionStringWithInvalidDataFails()
        {
            var config = SqliteConfigModel()
                .WithDefaults()
                // * | ? are invalid characters for a filename
                .WithConnectionString("Data Source=.\\*|?.db\"");

            BadRequestException exception = Assert.Throws<BadRequestException>(
                () => _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config));

            Assert.That(exception.Message, Is.EqualTo("Requested config values aren't valid"));
        }

        [Test]
        public void DatabasePlaceholderGetsReplaced()
        {
            var config = SqliteConfigModel()
                .WithDatabase("MyDatabase")
                .WithConnectionString("Data Source=.\\<DatabaseName>.db");

            _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config);
            var updatedConfig = GetUpdatedConfig();

            Assert.That(updatedConfig.ConfiguratorTypename, Does.Contain(_configuratorType.FullName));
            Assert.That(updatedConfig.ConnectionSettings.Database, Is.EqualTo("MyDatabase"));
            Assert.That(updatedConfig.ConnectionSettings.ConnectionString, Is.EqualTo("Data Source=.\\MyDatabase.db"));
        }

        [Test(Description = "Using <DatabaseName> placeholder in ConnectionString without setting Database property, leads to filling with DbContext type name.")]
        public void UsingDatabasePlaceholderWithoutDatabaseNameDefaultsToDbContextName()
        {
            // Arrange
            var config = SqliteConfigModel()
                .WithDatabase("")
                .WithConnectionString("Data Source=.\\<DatabaseName>.db");

            // Act
            _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config);

            // Assert
            var updatedConfig = GetUpdatedConfig();
            Assert.That(updatedConfig.ConnectionSettings.ConnectionString, Is.EqualTo($"Data Source=.\\{nameof(TestModelContext)}.db"));
        }

        private static DatabaseConfigModel SqliteConfigModel()
        {
            return new DatabaseConfigModel
            {
                ConfiguratorTypename = typeof(SqliteModelConfigurator).AssemblyQualifiedName,
                Entries = new() { }
            };
        }

        private static ConfigManager CreateConfigManager()
        {
            var configManager = new ConfigManager
            {
                ConfigDirectory = ""
            };
            return configManager;
        }

        private SqliteDatabaseConfig GetUpdatedConfig()
        {
            var configurator = _dbContextManager.GetConfigurator(typeof(TestModelContext));
            return (SqliteDatabaseConfig)configurator.Config;
        }
    }

    public static class DatabaseConfigModelBuilderExtension
    {
        public static DatabaseConfigModel WithDatabase(this DatabaseConfigModel model, string databaseName)
        {
            model.Entries["Database"] = databaseName;
            return model;
        }

        public static DatabaseConfigModel WithConnectionString(this DatabaseConfigModel model, string connectionString)
        {
            model.Entries["ConnectionString"] = connectionString;
            return model;
        }

        public static DatabaseConfigModel WithDefaults(this DatabaseConfigModel model)
        {
            model.Entries["ConnectionString"] = "Data Source=.\\db-filename.db";
            model.Entries["Database"] = "";
            return model;
        }
    }
}
