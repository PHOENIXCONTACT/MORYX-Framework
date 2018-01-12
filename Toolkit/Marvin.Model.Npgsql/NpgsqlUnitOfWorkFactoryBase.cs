using System.Data.Entity.Migrations;
using Marvin.Configuration;

namespace Marvin.Model.Npgsql
{
    /// <summary>
    /// Factory to get a unit of work 
    /// </summary>
    public abstract class NpgsqlUnitOfWorkFactoryBase<TContext> : UnitOfWorkFactoryBase<TContext>
        where TContext : MarvinDbContext
    {
        /// <summary>
        /// Config manager to load configuration of the model
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Returns the current migration configuration
        /// </summary>
        protected abstract DbMigrationsConfiguration<TContext> MigrationConfiguration { get; }

        /// <inheritdoc />
        protected sealed override IModelConfigurator CreateConfigurator()
        {
            return new NpgsqlModelConfigurator(this, ConfigManager, MigrationConfiguration);
        }
    }
}