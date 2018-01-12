using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    [Table("Another", Schema = "anotherschema")]
    public class AnotherEntity : ModificationTrackedEntityBase
    {

    }
}
