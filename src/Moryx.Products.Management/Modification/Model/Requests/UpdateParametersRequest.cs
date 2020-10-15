using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.Products.Management.Modification.Model
{
    [DataContract]
    internal class UpdateParametersRequest
    {
        [DataMember]
        public string Importer { get; set; }

        [DataMember]
        public Entry CurrentParameters { get; set; }
    }
}
