using System.Runtime.Serialization;
using Marvin.Model;

namespace Marvin.Workflows
{
    /// <summary>
    /// Step that was modified on the server
    /// </summary>
    [DataContract]
    public class ModifiedStep
    {
        /// <summary>
        /// The typ of modification
        /// </summary>
        [DataMember]
        public ModificationType Modification { get; set; }

        /// <summary>
        /// Step that was modified
        /// </summary>
        [DataMember]
        public WorkplanStep Data { get; set; }
    }
}