// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;

namespace Moryx.Model.Sqlite
{
    /// <summary>
    /// SQLite implementation of the <see cref="IDbContextManager"/>
    /// </summary>
    public sealed class SqliteDbContextManager : IDbContextManager
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _sqliteConnection;

        /// <summary>
        /// Creates a new instance of the <see cref="SqliteDbContextManager "/>.
        /// An SQLite factory will be created with the given connection string.
        /// </summary>
        public SqliteDbContextManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SqliteDbContextManager "/>.
        /// An SQLite factory will be created with the given SQLite connection.
        /// </summary>
        public SqliteDbContextManager(SqliteConnection sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Type> Contexts => Array.Empty<Type>();

        /// <inheritdoc />
        public IModelConfigurator GetConfigurator(Type contextType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IModelSetupExecutor GetSetupExecutor(Type contextType)
        {
            throw new NotImplementedException();
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
            var context = (TContext)Activator.CreateInstance(typeof(TContext), options);
            return context;
        }

        /// <inheritdoc />
        public void UpdateConfig(Type dbContextType, Type configuratorType, IDatabaseConfig databaseConfig)
        {
            throw new NotImplementedException();
        }
    }
}
