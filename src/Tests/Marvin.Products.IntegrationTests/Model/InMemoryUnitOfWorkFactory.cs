using Marvin.Model.InMemory;
using Marvin.Products.Model;
using Marvin.Products.Samples.Model;

namespace Marvin.Products.IntegrationTests
{
    public class InMemoryUnitOfWorkFactory : InMemoryUnitOfWorkFactoryBase<WatchProductsContext>
    {
        public InMemoryUnitOfWorkFactory(string instanceId) : base(instanceId)
        {
        }

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
            RegisterRepository<IStepEntityRepository>();
            RegisterRepository<IWorkplanEntityRepository>();
            RegisterRepository<IWorkplanReferenceRepository>();

            RegisterRepository<ISmartWatchProductPropertiesEntityRepository>();
            RegisterRepository<IAnalogWatchProductPropertiesEntityRepository>();
        }
    }
}
