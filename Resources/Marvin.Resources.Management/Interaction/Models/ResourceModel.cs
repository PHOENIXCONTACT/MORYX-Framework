using System.Collections.Generic;
using System.Runtime.Serialization;
using Marvin.Serialization;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Base model used by details view and tree
    /// </summary>
    [DataContract(IsReference = true)]
    public class ResourceModel
    {
        /// <summary>
        /// Id of the resource.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Id of the parent resource.
        /// </summary>
        [DataMember]
        public long ParentId { get; set; }

        /// <summary>
        /// Name of the resource.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The local identifier of the resource.
        /// </summary>
        [DataMember]
        public string LocalIdentifier { get; set; }

        /// <summary>
        /// The global identifier of the recource.
        /// </summary>
        [DataMember]
        public string GlobalIdentifier { get; set; }

        /// <summary>
        /// Resource type model reference
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// A list of configs of the resource.
        /// </summary>
        [DataMember]
        public List<Entry> Properties { get; set; }

        /// <summary>
        /// User callable methods of the resource
        /// </summary>
        [DataMember]
        public MethodEntry[] Methods { get; set; }

        /// <summary>
        /// References of this resource, including children
        /// </summary>
        [DataMember]
        public ResourceReferenceModel[] References { get; set; }
    }
}