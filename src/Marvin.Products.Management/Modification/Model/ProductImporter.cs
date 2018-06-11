using System.Runtime.Serialization;
using Marvin.Products.Management.Importers;
using Marvin.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract]
    internal class ProductImporter
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ImportParameter[] Parameters { get; set; }
    }
}