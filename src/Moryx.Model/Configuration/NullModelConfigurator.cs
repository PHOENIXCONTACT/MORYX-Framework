// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Null implementation of the <see cref="ModelConfiguratorBase{TConfig}"/>
    /// </summary>
    public sealed class NullModelConfigurator : IModelConfigurator
    {
        /// <inheritdoc />
        public IDatabaseConfig Config => null;

        /// <inheritdoc />
        public void Initialize(Type contextType, IConfigManager configManager, ILogger logger)
        {
        }

        /// <inheritdoc />
        public DbContext CreateContext()
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public DbContext CreateContext(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void UpdateConfig()
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<TestConnectionResult> TestConnection(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<bool> CreateDatabase(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<string>> AvailableMigrations(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<string>> AppliedMigrations(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task<DatabaseMigrationSummary> MigrateDatabase(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task DeleteDatabase(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task DumpDatabase(IDatabaseConfig config, string targetPath)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public Task RestoreDatabase(IDatabaseConfig config, string filePath)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }
    }
}