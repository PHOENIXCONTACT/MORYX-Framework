// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <summary>
    /// Result enum for function TestConnection
    /// </summary>
    public enum TestConnectionResult
    {
        /// <summary>
        /// Anything that belongs to a non existent or wrong configuration
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
        IDatabaseConfig Config { get; }

        /// <summary>
        /// Initializes the model configurator
        /// </summary>
        void Initialize(Type contextType, IConfigManager configManager, ILogger logger);

        /// <summary>
        /// Creates a context with internal configuration
        /// </summary>
        DbContext CreateContext();

        /// <summary>
        /// Creates a context based on a config
        /// </summary>
        DbContext CreateContext(IDatabaseConfig config);

        /// <summary>
        /// Creates a context based on `DbContextOptions`
        /// </summary>
        DbContext CreateContext(Type contextType, DbContextOptions dbContextOptions);

        /// <summary>
        /// Updates the configuration of the underlying model
        /// </summary>
        void UpdateConfig();

        /// <summary>
        /// Test connection for config
        /// </summary>
        Task<TestConnectionResult> TestConnection(IDatabaseConfig config);

        /// <summary>
        /// Create a new database for this model with given config
        /// </summary>
        Task<bool> CreateDatabase(IDatabaseConfig config);

        /// <summary>
        /// Retrieves all names of available updates
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<string>> AvailableMigrations(IDatabaseConfig config);

        /// <summary>
        /// Retrieves all names of installed updates
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<string>> AppliedMigrations(IDatabaseConfig config);

        /// <summary>
        ///
        /// </summary>
        Task<DatabaseMigrationSummary> MigrateDatabase(IDatabaseConfig config);

        /// <summary>
        /// Delete this database
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        Task DeleteDatabase(IDatabaseConfig config);

        /// <summary>
        /// Dump the database und save the backup at the given file path
        /// This method works asynchronous
        /// </summary>
        /// <param name="config">Config describing the database target</param>
        /// <param name="targetPath">Path to store backup</param>
        /// <returns>True if Backup is in progress</returns>
        Task DumpDatabase(IDatabaseConfig config, string targetPath);

        /// <summary>
        /// Restore this database with the given backup file
        /// </summary>
        /// <param name="config">Config to use</param>
        /// <param name="filePath">Filepath of dump</param>
        Task RestoreDatabase(IDatabaseConfig config, string filePath);
    }
}
