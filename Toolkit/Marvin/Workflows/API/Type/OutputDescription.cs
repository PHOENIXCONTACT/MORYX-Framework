using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Representation of each output index
    /// </summary>
    [DataContract]
    public class OutputDescription
    {
        /// <summary>
        /// Flag if this output can be considered a positive result
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// Name of this result
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Value used to compare a result object in order to find the right output
        /// </summary>
        [DataMember]
        public long MappingValue { get; set; }
    }
}