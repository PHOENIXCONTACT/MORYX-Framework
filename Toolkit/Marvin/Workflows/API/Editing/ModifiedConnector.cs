using System.Runtime.Serialization;
using Marvin.Model;

namespace Marvin.Workflows
{
    /// <summary>
    /// Connector that was modified on the server
    /// </summary>
    [DataContract]
    public class ModifiedConnector
    {
        /// <summary>
        /// The typ of modification
        /// </summary>
        [DataMember]
        public ModificationType Modification { get; set; }

        /// <summary>
        /// Connector that was modified
        /// </summary>
        [DataMember]
        public ConnectorDto Data { get; set; }

        /// <summary>
        /// Source of the connection
        /// </summary>
        [DataMember]
        public ConnectionPoint Source { get; set; }

        /// <summary>
        /// Target of the connection
        /// </summary>
        [DataMember]
        public ConnectionPoint Target { get; set; }
    }
}