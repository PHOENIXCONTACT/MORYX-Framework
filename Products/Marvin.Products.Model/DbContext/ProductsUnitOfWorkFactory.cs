using System.Data.Entity.Migrations;
using Marvin.Model;
using Marvin.Model.Npgsql;
using Marvin.Products.Model.Migrations;

namespace Marvin.Products.Model
{
    /// <summary>
    /// Factory to get a unit of work for the resources model
    /// </summary>
    public abstract class ProductsUnitOfWorkFactory<TContext> : NpgsqlUnitOfWorkFactoryBase<TContext>
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
        protected override DbMigrationsConfiguration<ProductsContext> MigrationConfiguration => new ProductMigrationsConfiguration();
    }
}