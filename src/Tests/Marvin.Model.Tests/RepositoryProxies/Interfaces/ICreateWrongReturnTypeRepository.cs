using System;

namespace Marvin.Model.Tests
{
    public interface ICreateWrongReturnTypeRepository : IRepository<SomeEntity>
    {
        Type Create(string wrong);
    }
}