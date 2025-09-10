﻿﻿using Moryx.Model.Sqlite;
using Moryx.Products.Model;
using Moryx.Runtime.Kernel;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace Moryx.Model.Tests
{
    [TestFixture]
    public class SqliteTests
    {
        private SqliteDatabaseConfig dbConfig;
        private SqliteModelConfigurator configurator;
        private string datasource;

        [SetUp]
        public void Setup()
        {
            string databaseName = "TestDatabase";
            datasource = Path.Combine(".", "db", databaseName+".db");
            string connectionString = $@"Data Source={datasource};";
            dbConfig = new SqliteDatabaseConfig();
            dbConfig.ConnectionSettings = new DatabaseConnectionSettings { ConnectionString = connectionString, Database = databaseName };
            configurator = new SqliteModelConfigurator();
            configurator.Initialize(typeof(ProductsContext), CreateConfigManager(), null);
        }

        [Test]
        public async Task SqliteCreateDatabaseShouldWork()
        {
            var result = await configurator.TestConnection(dbConfig);
            Assert.AreEqual(TestConnectionResult.ConnectionOkDbDoesNotExist, result);

            bool isCreated = await configurator.CreateDatabase(dbConfig);

            Assert.IsTrue(isCreated);
            Assert.IsTrue(File.Exists(datasource));

            //remove the database
            await configurator.DeleteDatabase(dbConfig);
        }


        [Test]
        public async Task SqliteDeleteDatabaseShouldWork()
        {
            var connectionResult = await configurator.TestConnection(dbConfig);
            Assert.AreEqual(TestConnectionResult.ConnectionOkDbDoesNotExist, connectionResult);

            bool isCreated = await configurator.CreateDatabase(dbConfig);
            Assert.IsTrue(isCreated);

            await configurator.DeleteDatabase(dbConfig);
            Assert.IsFalse(File.Exists(datasource));
        }

        private static ConfigManager CreateConfigManager()
        {
            var configManager = new ConfigManager
            {
                ConfigDirectory = ""
            };
            return configManager;
        }

        [TearDown]
        public void Destroy()
        {
            if (File.Exists(datasource))
                //remove the database
                configurator.DeleteDatabase(dbConfig).Wait();
        }
    }
}