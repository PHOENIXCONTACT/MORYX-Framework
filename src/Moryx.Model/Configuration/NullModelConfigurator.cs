// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Null implementation of the <see cref="ModelConfiguratorBase{TConfig}"/>
    /// </summary>
    public sealed class NullModelConfigurator : IModelConfigurator
    {
        /// <inheritdoc />
        public DatabaseConfig Config => new DatabaseConfig<DatabaseConnectionSettings>();

        /// <inheritdoc />
        public void Initialize(Type contextType, DatabaseConfig config, ILogger logger)
        {
        }

        /// <inheritdoc />
        public DbContext CreateContext()
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public DbContext CreateContext(DatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<TestConnectionResult> TestConnectionAsync(DatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<bool> CreateDatabaseAsync(DatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<string>> AvailableMigrationsAsync(DatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<string>> AppliedMigrationsAsync(DatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<DatabaseMigrationSummary> MigrateDatabaseAsync(DatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task DeleteDatabaseAsync(DatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public DbContext CreateContext(Type contextType, DbContextOptions dbContextOptions)
        {
            throw new NotImplementedException();
        }
    }
}
