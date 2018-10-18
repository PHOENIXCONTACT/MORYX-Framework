namespace Marvin.Model.Tests
{
    public interface ICreateAllParamsRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(int value, string name, int value2, int value3, int value4);
    }
}
