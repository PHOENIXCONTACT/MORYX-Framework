using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.Products.Model
{
    /// <summary>
    /// Factory to get a unit of work for the resources model
    /// </summary>
    public abstract class ProductsUnitOfWorkFactory<TContext> : UnitOfWorkFactoryBase<TContext, NpgsqlModelConfigurator>
        where TContext : ProductsContext
    {
        /// <inheritdoc />
        protected override void Configure()
        {
            RegisterRepository<IArticleEntityRepository>();
            RegisterRepository<IConnectorEntityRepository>();
            RegisterRepository<IConnectorReferenceRepository>();
            RegisterRepository<IOutputDescriptionEntityRepository>();
            RegisterRepository<IPartLinkRepository>();
            RegisterRepository<IProductDocumentRepository>();
            RegisterRepository<IProductEntityRepository>();
            RegisterRepository<IProductPropertiesRepository>();
            RegisterRepository<IProductRecipeEntityRepository>();
            RegisterRepository<IRevisionHistoryRepository>();
            RegisterRepository<IStepEntityRepository>();
            RegisterRepository<IWorkplanEntityRepository>();
            RegisterRepository<IWorkplanReferenceRepository>();
        }
    }

    [ModelFactory(ProductsConstants.Namespace)]
    public class ProductUnitOfWorkFactory : ProductsUnitOfWorkFactory<ProductsContext>
    {
    }
}