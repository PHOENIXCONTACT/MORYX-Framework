// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Serialization
{
    /// <summary>
    /// Model class representing a single property of a config class
    /// </summary>
    [DataContract]
    public class Entry : ICloneable
    {
        /// <summary>
        /// Identifier used for entry objects that function as prototypes
        /// </summary>
        public const string CreatedIdentifier = "CREATED";

        /// <summary>
        /// Create new entry instance with prefiled properties
        /// </summary>
        public Entry()
        {
            Value = new EntryValue();
            SubEntries = [];
            Prototypes = [];
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
        public Entry Instantiate()
        {
            var instance = Clone(true);
            instance.Identifier = CreatedIdentifier;
            return instance;
        }

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
        /// Compare two entries recursive to detect changes
        /// </summary>
        public static bool ValuesEqual(Entry left, Entry right)
        {
            if (left == null || right == null)
                return false;

            if (ReferenceEquals(left, right))
                return true;

            if (left.Value.Current != right.Value.Current)
                return false;

            if (left.SubEntries.Count != right.SubEntries.Count)
                return false;

            foreach (var leftEntry in left.SubEntries)
            {
                var rightEntry = right.SubEntries.FirstOrDefault(p => p.Identifier == leftEntry.Identifier);
                if (rightEntry == null)
                    return false;

                if (!ValuesEqual(leftEntry, rightEntry))
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{DisplayName}: {Value.Current}";
    }
}
