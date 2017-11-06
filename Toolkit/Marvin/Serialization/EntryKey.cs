using System;
using System.Runtime.Serialization;

namespace Marvin.Serialization
{
    /// <summary>
    /// Key of an config entry
    /// </summary>
    [DataContract]
    public partial class EntryKey : ICloneable
    {
        /// <summary>
        /// Name of the item - property name or list item name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Unique identfier - property name or item index
        /// </summary>
        [DataMember]
        public string Identifier { get; set; }

        /// <summary>
        /// Identifier used for entry objects that function as prototypes
        /// </summary>
        public const // TODO: Additional line: Wait for CGbR fix of constant https://github.com/Toxantron/CGbR/issues/24
            string PrototypeIdentifier = "PROTOTYPE";

        /// <see cref="ICloneable"/>
        public object Clone()
        {
            return Clone(true);
        }
    }
}