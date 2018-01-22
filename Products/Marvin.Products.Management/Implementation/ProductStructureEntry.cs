using System.Runtime.Serialization;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Management
{
    [DataContract(IsReference = true)]
    internal class ProductStructureEntry
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string MaterialNumber { get; set; }

        [DataMember]
        public short Revision { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public ProductState State { get; set; }

        [DataMember]
        public BranchType BranchType { get; set; }

        [DataMember]
        public ProductStructureEntry[] Branches { get; set; }
    }

    internal enum BranchType
    {
        Group,
        Product,
        PartCollector
    }
}