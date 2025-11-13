using System.Runtime.Serialization;

namespace Moryx.Serialization
{
    /// <summary>
    /// Possible value for an entry.
    /// </summary>
    [DataContract]
    public class EntryPossible
    {
        /// <summary>
        /// Invariant key of the possible value.
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Display name of the possible value.
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of the possible value.
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}