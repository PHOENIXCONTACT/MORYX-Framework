// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Repositories;
using Moryx.Model.Sqlite;

namespace Moryx.AbstractionLayer.TestTools
{
    /// <summary>
    /// Helper to create in memory db contexts
    /// </summary>
    public static class InMemoryUnitOfWorkFactoryBuilder
    {

        /// <summary>
        /// Instanciate a `UnitOfWorkFactory` of type `T`
        /// </summary>
        public static UnitOfWorkFactory<T> Sqlite<T>() where T : DbContext
        {

            // The in memory tests using SQLite need a permanently opened connection. Otherwise,
            // opening a database connection would result in creating a new in memory database
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = ":memory:",
            };

            var inMemoryDbConnection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            inMemoryDbConnection.Open();

            return new UnitOfWorkFactory<T>(new SqliteDbContextManager(inMemoryDbConnection));
        }

        /// <summary>
        /// Ensure that the database for the given UnitOfWork's `DbContext` is created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static UnitOfWorkFactory<T> EnsureDbIsCreated<T>(this UnitOfWorkFactory<T> factory)
            where T : DbContext
        {
            using var uow = factory.Create();
            uow.DbContext.Database.EnsureCreated();
            return factory;
        }
    }
}
