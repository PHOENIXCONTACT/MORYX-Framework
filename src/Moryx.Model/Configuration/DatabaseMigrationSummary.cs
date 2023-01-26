// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Result object created from running a database update
    /// </summary>
    public class DatabaseMigrationSummary
    {
        /// <summary>
        /// Flag that any updates were performed
        /// </summary>
        public MigrationResult Result { get; set; }

        /// <summary>
        /// All updates that were executed
        /// </summary>
        public string[] ExecutedMigrations { get; set; }
    }

    /// <summary>
    /// Result enum for executing database migrations
    /// </summary>
    public enum MigrationResult
    {
        /// <summary>
        /// Database was migrated to the current version
        /// </summary>
        Migrated,

        /// <summary>
        /// There are no migrations available
        /// </summary>
        NoMigrationsAvailable,

        /// <summary>
        /// There was an error while executing the migration
        /// </summary>
        Error
    }
}