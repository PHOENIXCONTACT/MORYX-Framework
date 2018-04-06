using Marvin.Model.InMemory;

namespace Marvin.TestTools.Test.Model
{
    public class InMemoryUnitOfWorkFactory : InMemoryUnitOfWorkFactoryBase<TestModelContext>
    {
        public InMemoryUnitOfWorkFactory()
        {
            
        }

        public InMemoryUnitOfWorkFactory(string instanceId) : base(instanceId)
        {
        }
        
        protected override void Configure()
        {
            RegisterRepository<ICarEntityRepository>();
            RegisterRepository<ISportCarRepository, SportCarRepository>();
            RegisterRepository<IWheelEntityRepository>();
            RegisterRepository<IJsonEntityRepository>();
            RegisterRepository<IHouseEntityRepository, HouseEntityRepository>(noProxy: true);
        }
    }
}