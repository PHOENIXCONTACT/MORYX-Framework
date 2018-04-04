using Marvin.Model;
using Marvin.TestTools.Test.Model;

namespace Marvin.TestTools.Test.Inheritance.Model
{
    [ModelFactory(InheritedTestModelConstants.Name, TestModelConstants.Name)]
    public class InheritedTestModelUnitOfWorkFactory : TestModelUnitOfWorkFactory<InheritedTestModelContext>
    {
        protected override void Configure()
        {
            RegisterRepository<ISuperCarEntityRepository>();
        }
    }
}
