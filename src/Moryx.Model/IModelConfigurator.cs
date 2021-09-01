// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Moryx.Configuration;
using Moryx.Logging;
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
        void Initialize(Type contextType, IConfigManager configManager, IModuleLogger logger);

        /// <summary>
        /// Creates a context with internal configuration
        /// </summary>
        DbContext CreateContext();

        /// <summary>
        /// Creates a context based on a config
        /// </summary>
        DbContext CreateContext(IDatabaseConfig config);

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
    }
}
