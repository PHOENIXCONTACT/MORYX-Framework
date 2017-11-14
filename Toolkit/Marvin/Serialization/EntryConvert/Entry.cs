using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Marvin.Serialization
{
    /// <summary>
    /// Model class representing a single property of a config class
    /// </summary>
    [DataContract]
    public partial class Entry : ICloneable
    {
        /// <summary>
        /// Create new entry instance with prefilled properties
        /// </summary>
        public Entry()
        {
            Key = new EntryKey();
            Value = new EntryValue();
            SubEntries = new List<Entry>();
            Prototypes = new List<Entry>();
        }

        /// <summary>
        /// Unique key of the entry
        /// </summary>
        [DataMember]
        public EntryKey Key { get; set; }

        /// <summary>
        /// Description of the entry
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Value of the entry
        /// </summary>
        [DataMember]
        public EntryValue Value { get; set; }

        /// <summary>
        /// Validation information
        /// </summary>
        [DataMember]
        public EntryValidation Validation { get; set; }

        /// <summary>
        /// Subentries of this entry - properties or list items
        /// </summary>
        [DataMember]
        public List<Entry> SubEntries { get; set; }

        /// <summary>
        /// Prototypes that can be used to create subentries
        /// </summary>
        [DataMember]
        public List<Entry> Prototypes { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{Key.Name}: {Value.Current}";
        }

        /// <summary>
        /// Get prototype with this name
        /// </summary>
        public Entry GetPrototype(string typeName)
        {
            return (from prototype in Prototypes
                    let value = prototype.Value
                    where value.Current == typeName
                       || value.Type.ToString("G") == typeName
                    select prototype).FirstOrDefault();
        }

        /// <summary>
        /// Use this object as a prototype and create a new instance
        /// Identifier switches to 
        /// </summary>
        public Entry Instantiate()
        {
            return Clone(true);
        }

        /// <see cref="ICloneable"/>
        public object Clone()
        {
            return Clone(true);
        }
    }
}