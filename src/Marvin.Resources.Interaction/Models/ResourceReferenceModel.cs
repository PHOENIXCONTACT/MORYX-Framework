using System.Collections.Generic;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Interaction
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
        /// Targets of this reference
        /// </summary>
        [DataMember]
        public List<ResourceModel> Targets { get; set; }
    }
}