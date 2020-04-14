// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Configuration;
using Moryx.Logging;

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
        /// Target model of this configurator
        /// </summary>
        string TargetModel { get; }

        /// <summary>
        /// Gets the configuration of the underlying model
        /// </summary>
        IDatabaseConfig Config { get; }

        /// <summary>
        /// Initializes the model configurator
        /// </summary>
        void Initialize(IUnitOfWorkFactory unitOfWorkFactory, IConfigManager configManager, IModuleLogger logger);

        /// <summary>
        /// Builds the connection string for the database.
        /// The model name will be included into the connection string
        /// </summary>
        string BuildConnectionString(IDatabaseConfig config);

        /// <summary>
        /// Builds the connection string for the database.
        /// The model name is optional within the connection string
        /// </summary>
        string BuildConnectionString(IDatabaseConfig config, bool includeModel);

        /// <summary>
        /// Updates the configuration of the underlying model
        /// </summary>
        void UpdateConfig();

        /// <summary>
        /// Test connection for config
        /// </summary>
        TestConnectionResult TestConnection(IDatabaseConfig config);

        /// <summary>
        /// Create a new database for this model with given config
        /// </summary>
        bool CreateDatabase(IDatabaseConfig config);

        /// <summary>
        /// Update the current database to the newest version
        /// </summary>
        /// <returns>True when an update was executed, false when this is already the latest version</returns>
        DatabaseUpdateSummary MigrateDatabase(IDatabaseConfig config);

        /// <summary>
        /// Update the database to a the given migration version if available
        /// </summary>
        /// <returns>True when an update was executed, false when this is already the latest version</returns>
        DatabaseUpdateSummary MigrateDatabase(IDatabaseConfig config, string migrationId);

        /// <summary>
        /// Rolls back all migrations including the first migration
        /// </summary>
        /// <returns></returns>
        bool RollbackDatabase(IDatabaseConfig config);

        /// <summary>
        /// Retrieves all names of available updates
        /// </summary>
        /// <returns></returns>
        IEnumerable<DatabaseUpdateInformation> AvailableMigrations(IDatabaseConfig config);

        /// <summary>
        /// Retrieves all names of installed updates
        /// </summary>
        /// <returns></returns>
        IEnumerable<DatabaseUpdateInformation> AppliedMigrations(IDatabaseConfig config);

        /// <summary>
        /// Delete this database
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        void DeleteDatabase(IDatabaseConfig config);

        /// <summary>
        /// Dump the database und save the backup at the given file path
        /// This method works asynchronous
        /// </summary>
        /// <param name="config">Config describing the database target</param>
        /// <param name="targetPath">Path to store backup</param>
        /// <returns>True if Backup is in progress</returns>
        void DumpDatabase(IDatabaseConfig config, string targetPath);

        /// <summary>
        /// Restore this database with the given backup file
        /// </summary>
        /// <param name="config">Config to use</param>
        /// <param name="filePath">Filepath of dump</param>
        void RestoreDatabase(IDatabaseConfig config, string filePath);

        /// <summary>
        /// Get all setups for this model
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelSetup> GetAllSetups();

        /// <summary>
        /// Execute setup for this config
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="setup">Setup</param>
        /// <param name="setupData"></param>
        void Execute(IDatabaseConfig config, IModelSetup setup, string setupData);
    }
}
