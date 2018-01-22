using System.Collections.Generic;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// DTO that represents the reference of one resource with another
    /// </summary>
    [DataContract(IsReference = true)]
    public class ResourceReferenceModel
    {
        /// <summary>
        /// Name of the reference, usually name of the property
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Types that can be assigned to this reference
        /// </summary>
        [DataMember]
        public ResourceTypeModel[] SupportedTypes { get; set; }

        /// <summary>
        /// Resources that could be assigned to this reference
        /// </summary>
        [DataMember]
        public ResourceModel[] PossibleTargets { get; set; }

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

        /// <summary>
        /// Flag if this reference is a collection
        /// </summary>
        [DataMember]
        public bool IsCollection { get; set; }

        /// <summary>
        /// Targets of this reference
        /// </summary>
        [DataMember]
        public List<ResourceModel> Targets { get; set; }
    }
}