// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
    public class Entry : ICloneable, IEquatable<Entry>
    {
        /// <summary>
        /// Identifier used for entry objects that function as prototypes
        /// </summary>
        public const string CreatedIdentifier = "CREATED";

        /// <summary>
        /// Create new entry instance with prefilled properties
        /// </summary>
        public Entry()
        {
            Value = new EntryValue();
            SubEntries = new List<Entry>();
            Prototypes = new List<Entry>();
        }

        /// <summary>
        /// Name of the item - property name or list item name
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Unique identifier - property name or item index
        /// </summary>
        [DataMember]
        public string Identifier { get; set; }

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
        /// Method to create a deep or shallow copy of this object
        /// </summary>
        public Entry Clone(bool deep)
        {
            var copy = new Entry
            {
                DisplayName = DisplayName, 
                Identifier = Identifier, 
                Description = Description
            };

            // All value types can be simply copied
            if (deep)
            {
                // In a deep clone the references are cloned 
                if (Value != null)
                {
                    copy.Value = Value.Clone(true);
                }
                if (Validation != null)
                {
                    copy.Validation = Validation.Clone(true);
                }

                if (SubEntries != null)
                {
                    var tempSubEntries = new List<Entry>(SubEntries.Count);
                    for (var i = 0; i < SubEntries.Count; i++)
                    {
                        var value = SubEntries[i];
                        if (value != null)
                        {
                            value = value.Clone(true);
                        }
                        tempSubEntries.Add(value);
                    }
                    copy.SubEntries = tempSubEntries;
                }

                if (Prototypes != null)
                {
                    var tempPrototypes = new List<Entry>(Prototypes.Count);
                    for (var i = 0; i < Prototypes.Count; i++)
                    {
                        var value = Prototypes[i];
                        if (value != null)
                        {
                            value = value.Clone(true);
                        }
                        tempPrototypes.Add(value);
                    }
                    copy.Prototypes = tempPrototypes;
                }
            }
            else
            {
                // In a shallow clone only references are copied
                copy.Value = Value;
                copy.Validation = Validation;
                copy.SubEntries = SubEntries;
                copy.Prototypes = Prototypes;
            }
            return copy;
        }

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
                var rightEntry = other.SubEntries.FirstOrDefault(p => p.Identifier == leftEntry.Identifier);
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
                // These values will not be modified
                var hashCode = Identifier.GetHashCode();
                hashCode = (hashCode * 397) ^ Value.Current.GetHashCode();
                hashCode = (hashCode * 397) ^ SubEntries.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString() => 
            $"{DisplayName}: {Value.Current}";
    }
}
