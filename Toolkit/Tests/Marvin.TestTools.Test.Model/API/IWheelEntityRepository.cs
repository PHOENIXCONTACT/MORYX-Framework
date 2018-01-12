using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    public interface IWheelEntityRepository : IRepository<WheelEntity>
    {
        WheelEntity Create(WheelType wheelType);

        WheelEntity Create(WheelType wheelType, CarEntity car);
    }
}
