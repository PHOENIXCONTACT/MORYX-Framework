// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Moryx.Configuration;
using Moryx.Logging;

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Base class for model configurators
    /// </summary>
    public abstract class ModelConfiguratorBase<TConfig> : IModelConfigurator
        where TConfig : class, IDatabaseConfig, new()
    {
        private IConfigManager _configManager;
        private string _configName;
        private Type _contextType;

        /// <summary>
        /// Logger for this model configurator
        /// </summary>
        protected IModuleLogger Logger { get; private set; }

        /// <inheritdoc />
        public IDatabaseConfig Config { get; private set; }

        /// <inheritdoc />
        public void Initialize(Type contextType, IConfigManager configManager, IModuleLogger logger)
        {
            _contextType = contextType;
            _configManager = configManager;

            // Add logger
            Logger = logger;

            // Load Config
            _configName = contextType.FullName + ".DbConfig";
            Config = _configManager.GetConfiguration<TConfig>(_configName);

            // If database is empty, fill with TargetModel name
            if (string.IsNullOrWhiteSpace(Config.Database))
                Config.Database = contextType.Name;
        }

        /// <inheritdoc />
        public DbContext CreateContext()
        {
            return CreateContext(Config);
        }

        /// <inheritdoc />
        public DbContext CreateContext(IDatabaseConfig config)
        {
            var context = (DbContext)Activator.CreateInstance(_contextType, BuildDbContextOptions(config));
            return context;
        }

        /// <inheritdoc />
        public void UpdateConfig()
        {
            _configManager.SaveConfiguration(Config, _configName);
        }

        /// <inheritdoc />
        public virtual TestConnectionResult TestConnection(IDatabaseConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Database))
                return TestConnectionResult.ConnectionError;

            if (!TestDatabaseConnection(config))
                return TestConnectionResult.ConnectionError;

            var context = CreateContext(config);
            try
            {
                return context.Database.CanConnect()
                    ? TestConnectionResult.Success
                    : TestConnectionResult.ConnectionOkDbDoesNotExist;
            }
            catch
            {
                return TestConnectionResult.ConnectionOkDbDoesNotExist;
            }
            finally
            {
                context.Dispose();
            }
        }

        /// <inheritdoc />
        public virtual bool CreateDatabase(IDatabaseConfig config)
        {
            // Check is database is configured
            if (!CheckDatabaseConfig(config))
            {
                return false;
            }

            using var context = CreateContext(config);
            context.Database.EnsureCreated();

            // Create connection to our new database
            var connection = CreateConnection(config);
            connection.Open();

            // Creation done -> close connection
            connection.Close();

            return true;
        }

        /// <summary>
        /// Creates a <see cref="DbConnection"/>
        /// </summary>
        protected abstract DbConnection CreateConnection(IDatabaseConfig config);

        /// <summary>
        /// Creates a <see cref="DbConnection"/>
        /// </summary>
        protected abstract DbConnection CreateConnection(IDatabaseConfig config, bool includeModel);

        /// <summary>
        /// Creates a <see cref="DbCommand"/>
        /// </summary>
        protected abstract DbCommand CreateCommand(string cmdText, DbConnection connection);

        /// <summary>
        /// Builds options to access the database
        /// </summary>
        public abstract DbContextOptions BuildDbContextOptions(IDatabaseConfig config);

        /// <inheritdoc />
        public abstract void DeleteDatabase(IDatabaseConfig config);

        /// <inheritdoc />
        public abstract void DumpDatabase(IDatabaseConfig config, string targetPath);

        /// <inheritdoc />
        public abstract void RestoreDatabase(IDatabaseConfig config, string filePath);

        /// <summary>
        /// Generally tests the connection to the database
        /// </summary>
        private bool TestDatabaseConnection(IDatabaseConfig config)
        {
            if (!CheckDatabaseConfig(config))
                return false;

            using var conn = CreateConnection(config, false);
            try
            {
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates the config gor Host, Database, Username and Port
        /// </summary>
        protected static bool CheckDatabaseConfig(IDatabaseConfig config)
        {
            return !(string.IsNullOrWhiteSpace(config.Host) ||
                     string.IsNullOrWhiteSpace(config.Database) ||
                     string.IsNullOrWhiteSpace(config.Username) ||
                     config.Port <= 0);
        }
    }
}
