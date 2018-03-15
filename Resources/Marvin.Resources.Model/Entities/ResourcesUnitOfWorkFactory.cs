using System.Data.Entity.Migrations;
using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.Resources.Model
{
    /// <summary>
    /// Factory to get a unit of work for the resources model
    /// </summary>
    [ModelFactory(ResourcesConstants.Namespace)]
    public class ResourcesUnitOfWorkFactory : NpgsqlUnitOfWorkFactoryBase<ResourcesContext>
    {
        /// <inheritdoc />
        protected override void Configure()
        {
            RegisterRepository<IResourceEntityRepository>();
            RegisterRepository<IResourceRelationRepository>();
        }

        /// <inheritdoc />
        protected override DbMigrationsConfiguration<ResourcesContext> MigrationConfiguration => new ResourcesMigrationsConfiguration();
    }
}