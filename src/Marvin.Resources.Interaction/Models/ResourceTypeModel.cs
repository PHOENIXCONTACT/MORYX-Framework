using System.Runtime.Serialization;
using Marvin.Serialization;

namespace Marvin.Resources.Interaction
{
    /// <summary>
    /// Type item for the tree
    /// </summary>
    [DataContract(IsReference = true)]
    public class ResourceTypeModel
    {
        /// <summary>
        /// The name of the resource type.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Display name of this resource type
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Flag that this type is registered and can be instantiated
        /// </summary>
        [DataMember]
        public bool Creatable { get; set; }

        /// <summary>
        /// Description of this resource type
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Resource constructors used to build new instances
        /// </summary>
        [DataMember]
        public MethodEntry[] Constructors { get; set; }

        /// <summary>
        /// Reference properties of this type
        /// </summary>
        [DataMember]
        public ReferenceTypeModel[] References { get; set; }

        /// <summary>
        /// Back reference to the base type
        /// </summary>
        [DataMember]
        public ResourceTypeModel BaseType { get; set; }

        /// <summary>
        /// Types derived from this type
        /// </summary>
        [DataMember]
        public ResourceTypeModel[] DerivedTypes { get; set; }
    }
}