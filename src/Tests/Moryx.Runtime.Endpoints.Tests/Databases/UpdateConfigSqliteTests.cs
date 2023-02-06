// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moq;
using Moryx.Model;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Services;
using Moryx.Runtime.Kernel;
using Moryx.TestTools.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Linq;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;
using Moryx.Model.Sqlite;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Moryx.Model.Configuration;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Exceptions;

namespace Moryx.Runtime.Endpoints.Tests.Databases
{
    internal class UpdateConfigSqliteTests
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

        [Test]
        public void LeavingDatabaseEmptyDefaultsToContextNamePasses()
        {
            var config = SqliteConfigModel()
                .WithDefaults();

            var result = _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config);

            var updatedConfig = GetUpdatedConfig();
            Assert.AreEqual(typeof(TestModelContext), result);
            Assert.True(updatedConfig.ConfiguratorTypename.Contains(_configuratorType.FullName));
            Assert.AreEqual("TestModelContext", updatedConfig.ConnectionSettings.Database);
            Assert.AreEqual("Data Source=.\\db-filename.db", updatedConfig.ConnectionSettings.ConnectionString);
        }

        [Test]
        public void LeavingConnectionStringEmptyFails()
        {
            var config = SqliteConfigModel()
                .WithDefaults()
                .WithConnectionString("");

            BadRequestException exception = Assert.Throws<BadRequestException>(
                () => _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config));

            Assert.AreEqual("Requested config values aren't valid", exception.Message);
        }

        [Test]
        public void InvalidConnectionStringFails()
        {
            var config = SqliteConfigModel()
                .WithDefaults()
                .WithConnectionString("rubbish");

            BadRequestException exception  = Assert.Throws<BadRequestException>(
                () => _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config));

            Assert.AreEqual("Requested config values aren't valid", exception.Message);
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

            Assert.AreEqual("Requested config values aren't valid", exception.Message);
        }

        [Test]
        public void DatabasePlaceholderGetsReplaced()
        {
            var config = SqliteConfigModel()
                .WithDatabase("MyDatabase")
                .WithConnectionString("Data Source=.\\<DatabaseName>.db");

            _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config);
            var updatedConfig = GetUpdatedConfig();

            Assert.True(updatedConfig.ConfiguratorTypename.Contains(_configuratorType.FullName));
            Assert.AreEqual("MyDatabase", updatedConfig.ConnectionSettings.Database);
            Assert.AreEqual("Data Source=.\\MyDatabase.db", updatedConfig.ConnectionSettings.ConnectionString);
        }

        [Test]
        public void UsingDatabasePlaceholderWithoutProvidingDatabasenameDefaultsToContextname()
        {
            var config = SqliteConfigModel()
                .WithDatabase("")
                .WithConnectionString("Data Source=.\\<DatabaseName>.db");

            _databaseConfigUpdateService.UpdateModel(_targetModelTypename, config);
            var updatedConfig = GetUpdatedConfig();

            Assert.AreEqual("Data Source=.\\TestModelContext.db", updatedConfig.ConnectionSettings.ConnectionString);
        }

        private static DatabaseConfigModel SqliteConfigModel()
        {
            return new DatabaseConfigModel
            {
                ConfiguratorTypename = typeof(SqliteModelConfigurator).AssemblyQualifiedName,
                Entries = new() { }
            };
        }

        private static ConfigManager CreateConfigManager() {
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
