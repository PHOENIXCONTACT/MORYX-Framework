using System.Collections.Generic;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Interaction
{
    /// <summary>
    /// Type model for a reference property on a resource type
    /// </summary>
    [DataContract]
    public class ReferenceTypeModel
    {
        /// <summary>
        /// Name of the reference, usually name of the property
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Flag if this reference is a collection
        /// </summary>
        [DataMember]
        public bool IsCollection { get; set; }

        /// <summary>
        /// Display name of the reference
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of this reference
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Reference is required and must be set
        /// </summary>
        [DataMember]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Types that can be assigned to this reference
        /// </summary>
        [DataMember]
        public string[] SupportedTypes { get; set; }

        /// <summary>
        /// Role of the referenced resource in the relationship
        /// </summary>
        [DataMember]
        public ResourceReferenceRole Role { get; set; }

        /// <summary>
        /// Type of resource relation
        /// </summary>
        [DataMember]
        public ResourceRelationType RelationType { get; set; }
    }
}