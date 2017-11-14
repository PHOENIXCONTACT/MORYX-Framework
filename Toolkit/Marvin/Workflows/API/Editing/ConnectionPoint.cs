using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Simple struct that represents a step id and the index in the array
    /// </summary>
    [DataContract]
    public struct ConnectionPoint
    {
        /// <summary>
        /// Indicator if this connection point is a special connector or a step
        /// </summary>
        [DataMember]
        public bool IsConnector { get; set; }

        /// <summary>
        /// Id of the step that is connected
        /// </summary>
        [DataMember]
        public long NodeId { get; set; }

        /// <summary>
        /// Index in the Inputs or Outputs array
        /// </summary>
        [DataMember]
        public int Index { get; set; }
    }
}