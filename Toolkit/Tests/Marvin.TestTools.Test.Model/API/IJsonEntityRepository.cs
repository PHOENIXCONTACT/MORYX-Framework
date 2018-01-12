using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    public interface IJsonEntityRepository : IRepository<JsonEntity>
    {
        JsonEntity Create(string jsonData);
    }
}
