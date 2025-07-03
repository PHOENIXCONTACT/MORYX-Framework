using System.Runtime.Serialization;

namespace Moryx.ControlSystem.VisualInstructions.Endpoints
{
    /// <summary>
    /// An instruction can have multiple items
    /// </summary>
    [DataContract]
    public class InstructionItemModel
    {
        /// <summary>
        /// Type of the <see cref="Content"/> property
        /// </summary>
        [DataMember]
        public InstructionContentType ContentType { get; set; }

        /// <summary>
        /// Content of the instruction item
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Content of the preview
        /// </summary>
        [DataMember]
        public string Preview { get; set; }
    }
}