using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.TestTools.Test.Model
{
    /// <summary>
    /// Used for inherited models
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class TestModelUnitOfWorkFactory<TContext> : UnitOfWorkFactoryBase<TContext, NpgsqlModelConfigurator>
        where TContext : TestModelContext
    {

    }

    /// <summary>
    /// Factory to get a unit of work for the TestModel model
    /// </summary>
    [ModelFactory(TestModelConstants.Name)]
    public sealed class TestModelUnitOfWorkFactory : TestModelUnitOfWorkFactory<TestModelContext>
    {
        protected override void Configure()
        {
            RegisterRepository<ICarEntityRepository>();
            RegisterRepository<ISportCarRepository, SportCarRepository>();
            RegisterRepository<IWheelEntityRepository>();
            RegisterRepository<IJsonEntityRepository>();
            RegisterRepository<IHouseEntityRepository, HouseEntityRepository>(true);
        }
    }
}  