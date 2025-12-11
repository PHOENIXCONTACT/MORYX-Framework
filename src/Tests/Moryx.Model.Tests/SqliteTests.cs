// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Sqlite;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using Moryx.TestTools.Test.Model;

namespace Moryx.Model.Tests
{
    [TestFixture]
    public class SqliteTests
    {
        private SqliteDatabaseConfig _dbConfig;
        private SqliteModelConfigurator _configurator;
        private string _dataSource;

        [SetUp]
        public void Setup()
        {
            var databaseName = "TestDatabase";
            _dataSource = Path.Combine(".", "db", databaseName + ".db");
            var connectionString = $@"Data Source={_dataSource};";
            _dbConfig = new SqliteDatabaseConfig
            {
                ConnectionSettings = new DatabaseConnectionSettings
                {
                    ConnectionString = connectionString,
                    Database = databaseName
                }
            };
            _configurator = new SqliteModelConfigurator();
            _configurator.Initialize(typeof(SqliteTestModelContext), _dbConfig, null);
        }

        [Test]
        public async Task SqliteCreateDatabaseShouldWork()
        {
            var result = await _configurator.TestConnectionAsync(_dbConfig);
            Assert.That(result, Is.EqualTo(TestConnectionResult.ConnectionOkDbDoesNotExist));

            var isCreated = await _configurator.CreateDatabaseAsync(_dbConfig);

            Assert.That(isCreated);
            Assert.That(File.Exists(_dataSource));

            //remove the database
            await _configurator.DeleteDatabaseAsync(_dbConfig);
        }

        [Test]
        public async Task SqliteDeleteDatabaseShouldWork()
        {
            var connectionResult = await _configurator.TestConnectionAsync(_dbConfig);
            Assert.That(connectionResult, Is.EqualTo(TestConnectionResult.ConnectionOkDbDoesNotExist));

            var isCreated = await _configurator.CreateDatabaseAsync(_dbConfig);
            Assert.That(isCreated);

            await _configurator.DeleteDatabaseAsync(_dbConfig);
            Assert.That(!File.Exists(_dataSource));
        }
        [TearDown]
        public void Destroy()
        {
            if (File.Exists(_dataSource))
                //remove the database
                _configurator.DeleteDatabaseAsync(_dbConfig).Wait();
        }
    }
}
