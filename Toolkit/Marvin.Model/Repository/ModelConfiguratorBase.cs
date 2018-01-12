using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Marvin.Container;

namespace Marvin.Model
{
    /// <summary>
    /// Base class for model configurators
    /// </summary>
    public abstract class ModelConfiguratorBase : IModelConfigurator
    {
        private readonly IContainer _container;
        private readonly IDbContextFactory _contextFactory;
        
        /// <summary>
        /// Current <see cref="IUnitOfWorkFactory"/> for the configurator
        /// </summary>
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

        /// <inheritdoc />
        public string TargetModel { get; }

        /// <inheritdoc />
        public IDatabaseConfig Config { get; protected set; }

        /// <summary>
        /// Constructor which initializes the model configurator
        /// </summary>
        protected ModelConfiguratorBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
            _contextFactory = unitOfWorkFactory as IDbContextFactory;
            if (_contextFactory == null)
                throw new InvalidOperationException("Factory have to implement " + nameof(IDbContextFactory));

            //TODO: find better way
            var factoryAttr = unitOfWorkFactory.GetType().GetCustomAttribute<ModelFactoryAttribute>();
            if (factoryAttr == null)
                throw new InvalidOperationException("Factory has to be attibuted with the: " + nameof(ModelFactoryAttribute));

            TargetModel = factoryAttr.TargetModel;

            _container = new CastleContainer();
            _container.LoadComponents<IModelSetup>(FilterTypeByModelAttribute);
            _container.LoadComponents<IModelScript>(FilterTypeByModelAttribute);
        }

        private bool FilterTypeByModelAttribute(Type type)
        {
            var scriptAttr = type.GetCustomAttribute<ModelAttribute>();
            return scriptAttr != null && scriptAttr.TargetModel == TargetModel;
        }

        /// <inheritdoc />
        public abstract void UpdateConfig();

        /// <inheritdoc />
        public virtual bool TestConnection(IDatabaseConfig config)
        {
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
        public abstract DatabaseUpdateSummary UpdateDatabase(IDatabaseConfig config, string updateName);

        /// <inheritdoc />
        public abstract bool RollbackDatabase(IDatabaseConfig config);

        /// <inheritdoc />
        public abstract IEnumerable<DatabaseUpdateInformation> AvailableUpdates(IDatabaseConfig config);

        /// <inheritdoc />
        public abstract IEnumerable<DatabaseUpdateInformation> InstalledUpdates(IDatabaseConfig config);

        /// <inheritdoc />
        public abstract void DeleteDatabase(IDatabaseConfig config);

        /// <inheritdoc />
        public abstract void DumpDatabase(IDatabaseConfig config, string filePath);

        /// <inheritdoc />
        public abstract void RestoreDatabase(IDatabaseConfig config, string filePath);
        
        /// <inheritdoc />
        public IEnumerable<IModelSetup> GetAllSetups()
        {
            return _container.ResolveAll<IModelSetup>();
        }

        /// <inheritdoc />
        public IEnumerable<IModelScript> GetAllScripts()
        {
            return _container.ResolveAll<IModelScript>();
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
    }
}