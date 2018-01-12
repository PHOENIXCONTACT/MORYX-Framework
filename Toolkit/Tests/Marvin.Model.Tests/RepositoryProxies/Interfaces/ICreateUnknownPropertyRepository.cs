namespace Marvin.Model.Tests
{
    public interface ICreateUnknownPropertyRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(string unknown);
    }
}