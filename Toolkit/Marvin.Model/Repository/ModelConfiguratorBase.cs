using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using Marvin.Configuration;
using Marvin.Tools;

namespace Marvin.Model
{
    /// <summary>
    /// Base class for model configurators
    /// </summary>
    public abstract class ModelConfiguratorBase<TConfig> : IModelConfigurator
        where TConfig : class, IDatabaseConfig, new()
    {
        private IConfigManager _configManager;
        private IDbContextFactory _contextFactory;
        private IDictionary<Type, IModelSetup> _setupDict;
        private IDictionary<Type, IModelScript> _scriptDict;
        private string _configName;
        private DbMigrationsConfiguration _currentMigrationConfiguration;
        private string[] _localMigrations;

        /// <summary>
        /// Current <see cref="IUnitOfWorkFactory"/> for the configurator
        /// </summary>
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; private set; }

        /// <summary>
        /// The invariant name of the database provider
        /// </summary>
        protected abstract string ProviderInvariantName { get; }

        /// <inheritdoc />
        public string TargetModel { get; private set; }

        /// <inheritdoc />
        public IDatabaseConfig Config { get; private set; }

        /// <inheritdoc />
        public void Initialize(IUnitOfWorkFactory unitOfWorkFactory, IConfigManager configManager)
        {
            UnitOfWorkFactory = unitOfWorkFactory;

            _contextFactory = unitOfWorkFactory as IDbContextFactory;
            _configManager = configManager;

            // Check cast
            if (_contextFactory == null)
                throw new InvalidOperationException("Factory have to implement " + nameof(IDbContextFactory));

            // Set TargetModel
            var factoryAttr = unitOfWorkFactory.GetType().GetCustomAttribute<ModelFactoryAttribute>();
            if (factoryAttr == null)
                throw new InvalidOperationException("Factory has to be attibuted with the: " + nameof(ModelFactoryAttribute));

            TargetModel = factoryAttr.TargetModel;

            // Load Config
            _configName = TargetModel + ".DbConfig";
            Config = _configManager.GetConfiguration<TConfig>(_configName);

            // Load ModelSetups TODO: Load internals
            _setupDict = ReflectionTool.GetPublicClasses<IModelSetup>(FilterTypeByModelAttribute).ToDictionary(t => t, t => (IModelSetup)null);

            // Load ModelScripts TODO: Load internals
            _scriptDict = ReflectionTool.GetPublicClasses<IModelScript>(FilterTypeByModelAttribute).ToDictionary(t => t, t => (IModelScript)null);
        }

        private bool FilterTypeByModelAttribute(Type type)
        {
            var scriptAttr = type.GetCustomAttribute<ModelAttribute>();
            return scriptAttr != null && scriptAttr.TargetModel == TargetModel;
        }

        /// <inheritdoc />
        public void UpdateConfig()
        {
            _configManager.SaveConfiguration(Config, _configName);
        }

        /// <inheritdoc />
        public virtual bool TestConnection(IDatabaseConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Database))
                return false;

            var context = _contextFactory.CreateContext(config, ContextMode.AllOff);
            try
            {
                return context.Database.Exists();
            }
            catch
            {
                return false;
            }
            finally
            {
                context.Dispose();
            }
        }

        /// <inheritdoc />
        public virtual bool CreateDatabase(IDatabaseConfig config)
        {
            using (var context = _contextFactory.CreateContext(config, ContextMode.AllOff))
            {
                if (string.IsNullOrWhiteSpace(config.Database))
                {
                    return false;
                }

                if (!AvailableUpdates(config).Any())
                {
                    return false;
                }

                // Check if this database is present on the server
                var dbExists = context.Database.Exists();
                if (dbExists)
                {
                    return false;
                }

                context.Database.Create();

                // Create connection to our new database
                var connection = CreateConnection(config);
                connection.Open();

                // Execute additional scripts
                foreach (var script in GetAllScripts().Where(r => r.IsCreationScript))
                {
                    var databaseScript = CreateCommand(script.GetSql(), connection);
                    databaseScript.ExecuteNonQuery();
                }

                // Creation done -> close connection
                connection.Close();

                return true;
            }
        }

        /// <summary>
        /// Creates a <see cref="DbConnection"/>
        /// </summary>
        protected abstract DbConnection CreateConnection(IDatabaseConfig config);

        /// <summary>
        /// Creates a <see cref="DbCommand"/>
        /// </summary>
        protected abstract DbCommand CreateCommand(string cmdText, DbConnection connection);

        /// <inheritdoc />
        public string BuildConnectionString(IDatabaseConfig config)
        {
            return BuildConnectionString(config, true);
        }

        /// <inheritdoc />
        public abstract string BuildConnectionString(IDatabaseConfig config, bool includeModel);

        /// <inheritdoc />
        public DatabaseUpdateSummary UpdateDatabase(IDatabaseConfig config)
        {
            return UpdateDatabase(config, string.Empty);
        }

        /// <inheritdoc />
        public virtual DatabaseUpdateSummary UpdateDatabase(IDatabaseConfig config, string updateName)
        {
            var result = new DatabaseUpdateSummary();
            var localUpdateName = updateName;

            var availableUpdates = AvailableUpdates(config).ToList();
            if (string.IsNullOrEmpty(localUpdateName))
            {
                localUpdateName = availableUpdates.LastOrDefault()?.Name;
            }

            var isAvailable = availableUpdates.Any(databaseUpdateInformation => databaseUpdateInformation.Name == localUpdateName);
            if (isAvailable)
            {
                CreateDbMigrator(config).Update(localUpdateName);
                result.ExecutedUpdates = InstalledUpdates(config).Select(databaseUpdateInformation => new DatabaseUpdate
                {
                    Description = databaseUpdateInformation.Name
                }).ToArray();
                result.WasUpdated = true;
            }

            return result;
        }

        /// <inheritdoc />
        public bool RollbackDatabase(IDatabaseConfig config)
        {
            CreateDbMigrator(config).Update("0");
            return true;
        }

        /// <inheritdoc />
        public IEnumerable<DatabaseUpdateInformation> AvailableUpdates(IDatabaseConfig config)
        {
            // Local migrations cannot be changed at runtime and are not dependent to the config
            if (_localMigrations == null)
            {
                _localMigrations = CreateDbMigrator(config).GetLocalMigrations().ToArray();
            }

            return _localMigrations.Select(name => new DatabaseUpdateInformation
            {
                Name = name
            });
        }

        /// <inheritdoc />
        public IEnumerable<DatabaseUpdateInformation> InstalledUpdates(IDatabaseConfig config)
        {
            return CreateDbMigrator(config).GetDatabaseMigrations().Select(name => new DatabaseUpdateInformation
            {
                Name = name
            });
        }

        /// <inheritdoc />
        public abstract void DeleteDatabase(IDatabaseConfig config);

        /// <inheritdoc />
        public abstract void DumpDatabase(IDatabaseConfig config, string filePath);

        /// <inheritdoc />
        public abstract void RestoreDatabase(IDatabaseConfig config, string filePath);
        
        /// <inheritdoc />
        public IEnumerable<IModelSetup> GetAllSetups()
        {
            return GetOrCreateFromDict(_setupDict);
        }

        /// <inheritdoc />
        public IEnumerable<IModelScript> GetAllScripts()
        {
            return GetOrCreateFromDict(_scriptDict);
        }

        /// <inheritdoc />
        public void Execute(IDatabaseConfig config, IModelSetup setup, string setupData)
        {
            var context = _contextFactory.CreateContext(config, ContextMode.AllOn);
            using (var unitOfWork = ((IContextUnitOfWorkFactory)UnitOfWorkFactory).Create(context))
            {
                setup.Execute(unitOfWork, setupData);
            }
        }

        /// <summary>
        /// Creates the instance from the given IDictionary{Type, object}
        /// </summary>
        private static IEnumerable<T> GetOrCreateFromDict<T>(IDictionary<Type, T> dict)
        {
            foreach (var kvPair in dict)
            {
                if (kvPair.Value == null)
                {
                    dict[kvPair.Key] = (T)Activator.CreateInstance(kvPair.Key);
                    yield return dict[kvPair.Key];
                }
                else
                {
                    yield return kvPair.Value;
                }
            }
        }

        private DbMigrator CreateDbMigrator(IDatabaseConfig config)
        {
            var configuration = GetOrCreateDbMigrationsConfiguration();
            configuration.TargetDatabase = new DbConnectionInfo(BuildConnectionString(config), ProviderInvariantName);

            return new DbMigrator(configuration);
        }

        /// <summary>
        /// Returns or creates and returns the DbMigrationsConfiguration of the Model
        /// </summary>
        private DbMigrationsConfiguration GetOrCreateDbMigrationsConfiguration()
        {
            if (_currentMigrationConfiguration != null)
                return _currentMigrationConfiguration;

            var assemblyTypes = UnitOfWorkFactory.GetType().Assembly.GetTypes();
            var configuration = assemblyTypes.FirstOrDefault(t => typeof(DbMigrationsConfiguration).IsAssignableFrom(t));

            if (configuration == null)
                return null;

            _currentMigrationConfiguration = (DbMigrationsConfiguration)Activator.CreateInstance(configuration);
            return _currentMigrationConfiguration;
        }
    }

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
        public void Initialize(IUnitOfWorkFactory unitOfWorkFactory, IConfigManager configManager)
        {
           
        }

        /// <inheritdoc />
        public string BuildConnectionString(IDatabaseConfig config)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public string BuildConnectionString(IDatabaseConfig config, bool includeModel)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void UpdateConfig()
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public bool TestConnection(IDatabaseConfig config)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public bool CreateDatabase(IDatabaseConfig config)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public DatabaseUpdateSummary UpdateDatabase(IDatabaseConfig config)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public DatabaseUpdateSummary UpdateDatabase(IDatabaseConfig config, string updateName)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public bool RollbackDatabase(IDatabaseConfig config)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public IEnumerable<DatabaseUpdateInformation> AvailableUpdates(IDatabaseConfig config)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public IEnumerable<DatabaseUpdateInformation> InstalledUpdates(IDatabaseConfig config)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void DeleteDatabase(IDatabaseConfig config)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void DumpDatabase(IDatabaseConfig config, string filePath)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void RestoreDatabase(IDatabaseConfig config, string filePath)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public IEnumerable<IModelSetup> GetAllSetups()
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public IEnumerable<IModelScript> GetAllScripts()
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void Execute(IDatabaseConfig config, IModelSetup setup, string setupData)
        {
            throw new InvalidOperationException("Not supported by " + nameof(NullModelConfigurator));
        }
    }
}