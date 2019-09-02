using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Interaction
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
        /// Name of the resource.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Description of the resource
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Resource type model reference
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Indicates that this model does not represent the full
        /// resource but only its key properties
        /// </summary>
        [DataMember]
        public bool PartiallyLoaded { get; set; }

        /// <summary>
        /// A list of configs of the resource.
        /// </summary>
        [DataMember]
        public Entry Properties { get; set; }

        /// <summary>
        /// User callable methods for instances of this type
        /// </summary>
        [DataMember]
        public MethodEntry[] Methods { get; set; }

        /// <summary>
        /// References of this resource, including children
        /// </summary>
        [DataMember]
        public ResourceReferenceModel[] References { get; set; }

        /// <summary>
        /// Checks if resource and model are equal
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="serialization"></param>
        /// <returns></returns>
        internal bool DifferentFrom(Resource resource, ICustomSerialization serialization)
        {
            var different = resource.Name != Name ||
                           resource.Description != Description;
            if (different)
                return true;

            // Do not compare values that were not transmitted
            if (resource.Descriptor == null || PartiallyLoaded)
                return false;

            var resourceProperties = EntryConvert.EncodeObject(resource.Descriptor, serialization);
            return !Properties.Equals(resourceProperties);
        }
    }
}