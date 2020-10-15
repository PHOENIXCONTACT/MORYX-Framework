using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.Products.Management.Modification
{
    [DataContract]
    internal class ImportProductRequest
    {
        [DataMember]
        public string ImporterName { get; set; }

        [DataMember]
        public Entry ParametersModel { get; set; }
    }
}
