using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    [Table(nameof(CarEntity), Schema = "myschema")]
    public class CarEntity : ModificationTrackedEntityBase
    {
        public virtual string Name { get; set; }

        public virtual int Price { get; set; }

        public virtual byte[] Image { get; set; }

        public virtual ICollection<WheelEntity> Wheels { get; set; }
    }
}