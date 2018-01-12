using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    public class HouseEntity : ModificationTrackedEntityBase
    {
        public virtual string Name { get; set; }

        public virtual int Size { get; set; }

        public virtual bool IsMethLabratory { get; set; }

        public virtual bool IsBurnedDown { get; set; }

        //public virtual bool HasCellar { get; set; }
    }
}
