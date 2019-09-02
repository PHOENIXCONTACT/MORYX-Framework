using System.Runtime.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract]
    internal class DuplicateProductResponse
    {
        [DataMember]
        public bool IdentityConflict { get; set; }

        [DataMember]
        public bool InvalidSource { get; set; }

        [DataMember]
        public ProductModel Duplicate { get; set; }
    }
}