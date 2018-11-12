using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.TestTools.Test.Model
{
    /// <summary>
    /// Factory to get a unit of work for the TestModel model
    /// </summary>
    [ModelFactory(AnotherTestModelConstants.Namespace, TestModelConstants.Namespace)]
    public sealed class AnotherTestModelUnitOfWorkFactory : UnitOfWorkFactoryBase<AnotherTestModelContext, NpgsqlModelConfigurator>
    {
        protected override void Configure()
        {
            RegisterRepository<IAnotherEntityRepository>();
        }
    }
}