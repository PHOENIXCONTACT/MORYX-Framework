namespace Marvin.Model.Tests
{
    public interface ICreateValueParamRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(string value);
    }
}