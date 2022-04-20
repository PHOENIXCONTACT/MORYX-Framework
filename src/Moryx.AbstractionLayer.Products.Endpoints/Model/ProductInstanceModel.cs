using Moryx.Serialization;
using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints.Model
{
    [DataContract]
    public class ProductInstanceModel
    {
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// The name of the product type of this instance
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        /// <summary>
        /// The current state of the instance
        /// </summary>
        public ProductInstanceState State { get; set; }

        [DataMember]
        public Entry Properties { get; set; }
    }
}
