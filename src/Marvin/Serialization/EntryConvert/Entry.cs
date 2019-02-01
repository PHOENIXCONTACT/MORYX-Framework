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
    public partial class Entry : ICloneable, IEquatable<Entry>
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
        public Entry Instantiate() => 
            Clone(true);

        /// <see cref="ICloneable"/>
        public object Clone() => 
            Clone(true);

        /// <summary>
        /// Overload of the comparison operator mapped to the <see cref="Equals(Entry)"/> method
        /// </summary>
        public static bool operator ==(Entry left, Entry right)
        {
            if (ReferenceEquals(null, left))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        /// <summary>
        /// Overload of the comparison operator mapped to the <see cref="Equals(Entry)"/> method
        /// </summary>
        public static bool operator !=(Entry left, Entry right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public bool Equals(Entry other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Value.Current != other.Value.Current)
                return false;

            if (SubEntries.Count != other.SubEntries.Count)
                return false;

            foreach (var leftEntry in SubEntries)
            {
                var rightEntry = other.SubEntries.FirstOrDefault(p => p.Key.Name == leftEntry.Key.Name);
                if (rightEntry == null)
                    return false;

                if (!leftEntry.Equals(rightEntry))
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var entry = obj as Entry;
            return entry != null && Equals(entry);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                // These valuses will not be modified
                var hashCode = Key.Identifier.GetHashCode();
                hashCode = (hashCode * 397) ^ Value.Current.GetHashCode();
                hashCode = (hashCode * 397) ^ SubEntries.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString() => 
            $"{Key.Name}: {Value.Current}";
    }
}