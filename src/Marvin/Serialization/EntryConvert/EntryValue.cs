// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;

namespace Marvin.Serialization
{
    /// <summary>
    /// Value of an config entry
    /// </summary>
    [DataContract]
    public class EntryValue : ICloneable
    {
        /// <summary>
        /// Type of this entry
        /// </summary>
        [DataMember]
        public EntryValueType Type { get; set; }

        /// <summary>
        /// Unit type of this entry
        /// </summary>
        [DataMember]
        public EntryUnitType UnitType { get; set; }

        /// <summary>
        /// Current value of this entry
        /// </summary>
        [DataMember]
        public string Current { get; set; }

        /// <summary>
        /// Default value of this entry
        /// </summary>
        [DataMember]
        public string Default { get; set; }

        /// <summary>
        /// Possible values for this entry
        /// </summary>
        [DataMember]
        public string[] Possible { get; set; }

        /// <summary>
        /// Indicates wether the field is read only
        /// </summary>
        [DataMember]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Clone this entry value for an element of another value
        /// </summary>
        public EntryValue Clone(string newValue)
        {
            var clone = Clone(false);
            clone.Current = newValue;
            return clone;
        }

        /// <see cref="ICloneable"/>
        public object Clone()
        {
            return Clone(false);
        }

        /// <summary>
        /// Method to create a deep or shallow copy of this object
        /// </summary>
        public EntryValue Clone(bool deep)
        {
            // All value types can be simply copied
            var copy = new EntryValue
            {
                Type = Type,
                UnitType = UnitType,
                Current = Current,
                Default = Default,
                IsReadOnly = IsReadOnly
            };

            if (deep)
            {
                // In a deep clone the references are cloned 

                if (Possible != null)
                {
                    var tempPossible = new string[Possible.Length];
                    for (var i = 0; i < Possible.Length; i++)
                    {
                        var value = Possible[i];
                        tempPossible[i] = value;
                    }
                    copy.Possible = tempPossible;
                }
            }
            else
            {
                // In a shallow clone only references are copied
                copy.Possible = Possible;
            }

            return copy;
        }
    }
}
