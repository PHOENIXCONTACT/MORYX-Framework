using System.Runtime.Serialization;
using Marvin.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract(IsReference = true)]
    internal class PartModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Entry Properties { get; set; }

        [DataMember]
        public ProductModel Product { get; set; }
    }
}