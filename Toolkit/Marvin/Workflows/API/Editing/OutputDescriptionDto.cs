using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Representation of each output index
    /// </summary>
    [DataContract(Name = "OutputDescription")]
    public sealed class OutputDescriptionDto
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
    }
}