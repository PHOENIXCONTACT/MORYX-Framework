// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Resources.Endpoints
{
    /// <summary>
    /// Base model used by details view and tree
    /// </summary>
    [DataContract]
    public class ResourceModel
    {
        /// <summary>
        /// Id of the resource.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Reference id in case of recursion in the returned model
        /// </summary>
        [DataMember]
        public long ReferenceId { get; set; }

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
            // Do not compare values that were not transmitted
            if (resource.Descriptor == null || PartiallyLoaded)
                return false;

            // Compare obvious base properties
            var different = resource.Name != Name || resource.Description != Description;
            if (different)
                return true;

            var resourceProperties = EntryConvert.EncodeObject(resource.Descriptor, serialization);
            return !Entry.ValuesEqual(Properties, resourceProperties);
        }
    }
}
