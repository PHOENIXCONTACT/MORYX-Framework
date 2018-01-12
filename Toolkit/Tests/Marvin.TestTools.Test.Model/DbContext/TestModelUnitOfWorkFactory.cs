using System.Data.Entity.Migrations;
using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.TestTools.Test.Model
{
    /// <summary>
    /// Factory to get a unit of work for the TestModel model
    /// </summary>
    [ModelFactory(TestModelConstants.Name)]
    public class TestModelUnitOfWorkFactory : NpgsqlUnitOfWorkFactoryBase<TestModelContext>
    {
        protected override DbMigrationsConfiguration<TestModelContext> MigrationConfiguration => new Migrations.Configuration();

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