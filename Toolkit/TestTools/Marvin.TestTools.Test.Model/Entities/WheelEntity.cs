using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    public enum WheelType
    {
        FrontLeft,
        FrontRight,
        RearLeft,
        RearRight
    }

    public class WheelEntity : EntityBase
    {
        public virtual CarEntity Car { get; set; }

        public virtual WheelType WheelType { get; set; }
    }
}
