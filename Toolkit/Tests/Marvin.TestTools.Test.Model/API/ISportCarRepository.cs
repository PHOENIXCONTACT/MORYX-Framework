using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    public interface ISportCarRepository : IRepository<SportCarEntity>
    {
        SportCarEntity GetSingleBy(string name);
    }

    public class SportCarRepository : ModificationTrackedRepository<SportCarEntity>, ISportCarRepository
    {
        public virtual SportCarEntity GetSingleBy(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}