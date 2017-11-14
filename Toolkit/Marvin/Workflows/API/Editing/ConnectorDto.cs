using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Marvin.Workflows
{
    /// <summary>
    /// Dto for client editing communication
    /// </summary>
    [DataContract(Name = "Connector", IsReference = true)]
    public sealed class ConnectorDto
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

        /// <summary>
        /// Temporary id
        /// </summary>
        [DataMember]
        public string TemporaryId { get; set; }

        internal IConnector ToConnector()
        {
            return new Connector
            {
                Id = Id,
                Name = Name,
                Classification = Classification
            };
        }

        internal static ConnectorDto FromConnector(IConnector source)
        {
            return new ConnectorDto
            {
                Id = source.Id,
                Name = source.Name,
                Classification = source.Classification
            };
        }
    }
}