using System;
using Marvin.Model;
using Marvin.Model.InMemory;

namespace Marvin.TestTools.Test.Model
{
    [ModelFactory(TestModelConstants.Namespace)]
    public class InMemoryUnitOfWorkFactory : InMemoryUnitOfWorkFactoryBase<TestModelContext>
    {
        public InMemoryUnitOfWorkFactory() : base(Guid.NewGuid().ToString())
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
            RegisterRepository<IHugePocoRepository>();
            RegisterRepository<IHouseEntityRepository, HouseEntityRepository>(noProxy: true);
        }
    }
}