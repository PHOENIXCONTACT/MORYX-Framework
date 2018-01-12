namespace Marvin.Model.Tests
{
    public interface ICreateStringParamRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(string name);
    }
}
