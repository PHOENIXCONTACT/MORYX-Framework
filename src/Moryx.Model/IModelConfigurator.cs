// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <summary>
    /// Result enum for function TestConnection
    /// </summary>
    public enum TestConnectionResult
    {
        /// <summary>
        /// Anything that belongs to a non-existent or wrong configuration
        /// </summary>
        ConfigurationError,

        /// <summary>
        /// Connection could not be established
        /// </summary>
        ConnectionError,

        /// <summary>
        /// Connection could be established but database was not found
        /// </summary>
        ConnectionOkDbDoesNotExist,

        /// <summary>
        /// Connection to database was ok but the model have pending migrations
        /// </summary>
        PendingMigrations,

        /// <summary>
        /// Connection could be established but database was found
        /// </summary>
        Success
    }

    /// <summary>
    /// Interface for interaction with model
    /// </summary>
    public interface IModelConfigurator
    {
        /// <summary>
        /// Gets the configuration of the underlying model
        /// </summary>
        DatabaseConfig Config { get; }

        /// <summary>
        /// Initializes the model configurator
        /// </summary>
        void Initialize(Type contextType, DatabaseConfig config, ILogger logger);

        /// <summary>
        /// Creates a context with internal configuration
        /// </summary>
        DbContext CreateContext();

        /// <summary>
        /// Creates a context based on a config
        /// </summary>
        DbContext CreateContext(DatabaseConfig config);

        /// <summary>
        /// Creates a context based on `DbContextOptions`
        /// </summary>
        DbContext CreateContext(Type contextType, DbContextOptions dbContextOptions);

        /// <summary>
        /// Test connection for config
        /// </summary>
        Task<TestConnectionResult> TestConnectionAsync(DatabaseConfig config);

        /// <summary>
        /// Create a new database for this model with given config
        /// </summary>
        Task<bool> CreateDatabaseAsync(DatabaseConfig config);

        /// <summary>
        /// Retrieves all names of available updates
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<string>> AvailableMigrationsAsync(DatabaseConfig config);

        /// <summary>
        /// Retrieves all names of installed updates
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<string>> AppliedMigrationsAsync(DatabaseConfig config);

        /// <summary>
        ///
        /// </summary>
        Task<DatabaseMigrationSummary> MigrateDatabaseAsync(DatabaseConfig config);

        /// <summary>
        /// Delete this database
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        Task DeleteDatabaseAsync(DatabaseConfig config);
    }
}
