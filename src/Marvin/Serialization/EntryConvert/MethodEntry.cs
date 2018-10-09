using System;
using System.Runtime.Serialization;

namespace Marvin.Serialization
{
    /// <summary>
    /// Model that represents a public method on an object callable from the WCF service and UI
    /// </summary>
    [DataContract]
    public partial class MethodEntry : ICloneable
    {
        /// <summary>
        /// Name of the method
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Flag if this structure represents a server side constructor
        /// </summary>
        public bool IsConstructor { get; set; }

        /// <summary>
        /// Display name for the button
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of the method
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Arguments of the 
        /// </summary>
        [DataMember]
        public Entry Parameters { get; set; }

        /// <inheritdoc />
        public object Clone()
        {
            return Clone(true);
        }
    }
}