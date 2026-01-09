// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Modules;

namespace Moryx.Model.Configuration;

/// <summary>
/// Null implementation of the <see cref="ModelConfiguratorBase{TConfig}"/>
/// </summary>
[ExpectedConfig(typeof(NullDatabaseConfig))]
public sealed class NullModelConfigurator : IModelConfigurator
{
    /// <inheritdoc />
    public DatabaseConfig Config => new NullDatabaseConfig();

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
    public Task<TestConnectionResult> TestConnectionAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
    }

    /// <inheritdoc />
    public Task<bool> CreateDatabaseAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> AvailableMigrationsAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> AppliedMigrationsAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
    }

    /// <inheritdoc />
    public Task<DatabaseMigrationSummary> MigrateDatabaseAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
    }

    /// <inheritdoc />
    public Task DeleteDatabaseAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
    }

    /// <inheritdoc />
    public DbContext CreateContext(Type contextType, DbContextOptions dbContextOptions)
    {
        throw new NotImplementedException();
    }
}