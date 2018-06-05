using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Connector implementations
    /// </summary>
    [DataContract(IsReference = true)]
    public class Connector : IConnector
    {
        /// 
        [DataMember]
        public long Id { get; set; }

        ///
        [DataMember]
        public string Name { get; set; }

        /// 
        [DataMember]
        public NodeClassification Classification { get; set; }

        /// 
        public virtual IPlace CreateInstance()
        {
            return new Place
            {
                Id = Id,
                Classification = Classification
            };
        }
    }
}