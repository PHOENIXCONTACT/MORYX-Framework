using System.Runtime.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract]
    internal class StorageValue
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string[] Values { get; set; }
    }
}