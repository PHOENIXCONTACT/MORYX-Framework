using System;
using System.Collections.Generic;
using Moryx.Configuration;
using Moryx.Logging;

namespace Moryx.Model
{
    /// <summary>
    /// Null implementation of the <see cref="ModelConfiguratorBase{TConfig}"/>
    /// </summary>
    public sealed class NullModelConfigurator : IModelConfigurator
    {
        /// <inheritdoc />
        public string TargetModel => string.Empty;

        /// <inheritdoc />
        public IDatabaseConfig Config => null;

        /// <inheritdoc />
        public void Initialize(IUnitOfWorkFactory unitOfWorkFactory, IConfigManager configManager, IModuleLogger logger)
        {

        }

        /// <inheritdoc />
        public string BuildConnectionString(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public string BuildConnectionString(IDatabaseConfig config, bool includeModel)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void UpdateConfig()
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public TestConnectionResult TestConnection(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public bool CreateDatabase(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public DatabaseUpdateSummary MigrateDatabase(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public DatabaseUpdateSummary MigrateDatabase(IDatabaseConfig config, string migrationId)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public bool RollbackDatabase(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public IEnumerable<DatabaseUpdateInformation> AvailableMigrations(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public IEnumerable<DatabaseUpdateInformation> AppliedMigrations(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void DeleteDatabase(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void DumpDatabase(IDatabaseConfig config, string targetPath)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void RestoreDatabase(IDatabaseConfig config, string filePath)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public IEnumerable<IModelSetup> GetAllSetups()
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void Execute(IDatabaseConfig config, IModelSetup setup, string setupData)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }
    }
}