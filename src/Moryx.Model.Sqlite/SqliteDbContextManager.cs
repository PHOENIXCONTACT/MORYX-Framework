// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;
using Moryx.Runtime.Kernel;

namespace Moryx.Model.Sqlite
{
    /// <summary>
    /// SQLite implementation of the <see cref="IDbContextManager"/>
    /// </summary>
    public sealed class SqliteDbContextManager : IDbContextManager
    {
        private Dictionary<Type, IModelConfigurator> _configurators;
        private readonly string _connectionString;
        private readonly SqliteConnection _sqliteConnection;

        /// <summary>
        /// Creates a new instance of the <see cref="SqliteDbContextManager "/>.
        /// An SQLite factory will be created with the given connection string.
        /// </summary>
        public SqliteDbContextManager(string connectionString)
        {
            _connectionString = connectionString;
            _configurators = new Dictionary<Type, IModelConfigurator>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SqliteDbContextManager "/>.
        /// An SQLite factory will be created with the given SQLite connection.
        /// </summary>
        public SqliteDbContextManager(SqliteConnection sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
            _configurators = new Dictionary<Type, IModelConfigurator>();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Type> Contexts => _configurators.Select(c => c.Key).ToArray();

        /// <inheritdoc />
        public IModelConfigurator GetConfigurator(Type contextType)
        {
            return _configurators[contextType];
        }

        /// <inheritdoc />
        public IModelSetupExecutor GetSetupExecutor(Type contextType)
        {
            var setupExecutorType = typeof(ModelSetupExecutor<>).MakeGenericType(contextType);
            return (IModelSetupExecutor)Activator.CreateInstance(setupExecutorType, this);
        }

        /// <inheritdoc />
        public TContext Create<TContext>() where TContext : DbContext =>
            Create<TContext>(null);

        /// <inheritdoc />
        public TContext Create<TContext>(IDatabaseConfig config) where TContext : DbContext
        {
            var connectionString = string.IsNullOrEmpty(_connectionString)
                ? Guid.NewGuid().ToString()
                : _connectionString;

            DbContextOptions options;
            if (_sqliteConnection != null)
            {
                options = new DbContextOptionsBuilder<TContext>()
                    .UseSqlite(_sqliteConnection)
                    .Options;
            }
            else
            {
                options = new DbContextOptionsBuilder<TContext>()
                    .UseSqlite(connectionString)
                    .Options;
            }

            // Create instance of context
            var configurator = new SqliteModelConfigurator();
            configurator.Initialize(typeof(TContext), CreateConfigManager(), null);
            var context = (TContext)configurator.CreateContext(typeof(TContext), options);
            _configurators.TryAdd(context.GetType(), configurator);
            return context;
        }

        /// <inheritdoc />
        public void UpdateConfig(Type dbContextType, Type configuratorType, IDatabaseConfig databaseConfig)
        {
            throw new NotImplementedException();
        }

        private static ConfigManager CreateConfigManager()
        {
            var configManager = new ConfigManager
            {
                ConfigDirectory = ""
            };
            return configManager;
        }
    }
}
