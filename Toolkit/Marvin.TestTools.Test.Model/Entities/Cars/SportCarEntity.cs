using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.TestTools.Test.Model
{
    [Table(nameof(SportCarEntity), Schema = TestModelConstants.CarsSchema)]
    public class SportCarEntity : CarEntity
    {
        public virtual int Performance { get; set; }
    }
}