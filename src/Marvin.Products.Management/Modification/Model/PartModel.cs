using System.Runtime.Serialization;
using Marvin.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract(IsReference = true)]
    internal class PartModel
    {
        [DataMember]
        public Entry[] Properties { get; set; }

        [DataMember]
        public ProductModel Product { get; set; }
    }
}