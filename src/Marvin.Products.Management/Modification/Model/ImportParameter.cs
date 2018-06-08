using System.Runtime.Serialization;
using Marvin.Serialization;

namespace Marvin.Products.Management.Modification
{
    /// <summary>
    /// Specialized type of entry that supports updating the object instance
    /// </summary>
    [DataContract]
    internal class ImportParameter : Entry
    {
        /// <summary>
        /// Flag of this parameter triggers an update
        /// </summary>
        [DataMember]
        public bool TriggersUpdate { get; set; }
    }
}