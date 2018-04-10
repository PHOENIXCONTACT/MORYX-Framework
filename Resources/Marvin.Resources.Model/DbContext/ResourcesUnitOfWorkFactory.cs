using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.Resources.Model
{
    /// <summary>
    /// Factory to get a unit of work for the resources model
    /// </summary>
    [ModelFactory(ResourcesConstants.Namespace)]
    public class ResourcesUnitOfWorkFactory : UnitOfWorkFactoryBase<ResourcesContext, NpgsqlModelConfigurator>
    {
        /// <inheritdoc />
        protected override void Configure()
        {
            RegisterRepository<IResourceEntityRepository>();
            RegisterRepository<IResourceRelationRepository>();
        }
    }
}