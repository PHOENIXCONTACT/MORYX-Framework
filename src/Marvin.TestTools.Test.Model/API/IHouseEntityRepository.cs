using System.Collections.Generic;
using System.Linq;
using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    public interface IHouseEntityRepository : IRepository<HouseEntity>
    {
        ICollection<HouseEntity> GetMethLabratories();
    }

    public class HouseEntityRepository : ModificationTrackedRepository<HouseEntity>, IHouseEntityRepository
    {
        public virtual ICollection<HouseEntity> GetMethLabratories()
        {
            // This method should not be proxied
            return DbSet.Where(h => h.IsMethLabratory).ToList();
        }
    }
}
