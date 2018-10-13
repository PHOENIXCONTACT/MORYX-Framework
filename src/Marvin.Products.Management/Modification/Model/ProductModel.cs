using System.Runtime.Serialization;
using Marvin.AbstractionLayer;
using Marvin.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract(IsReference = true)]
    internal class ProductModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Identifier { get; set; }

        [DataMember]
        public short Revision { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public ProductState State { get; set; }

        [DataMember]
        public Entry Properties { get; set; }

        [DataMember]
        public ProductFile[] Files { get; set; }

        [DataMember]
        public PartConnector[] Parts { get; set; }

        [DataMember]
        public RecipeModel[] Recipes { get; set; }
    }
}