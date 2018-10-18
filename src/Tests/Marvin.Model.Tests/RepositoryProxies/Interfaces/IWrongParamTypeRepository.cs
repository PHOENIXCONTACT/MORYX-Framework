namespace Marvin.Model.Tests
{
    public interface IWrongParamTypeRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(int value, string name, long value2);
    }
}
