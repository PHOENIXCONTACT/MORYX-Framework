// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;

namespace Marvin.Serialization
{
    /// <summary>
    /// All validation rules of an entry
    /// </summary>
    [DataContract]
    public class EntryValidation : ICloneable
    {
        /// <summary>
        /// Minimal lenght of strings or other types
        /// </summary>
        [DataMember]
        public int MinLenght { get; set; }

        /// <summary>
        /// Maximal lenght of strings or other types
        /// </summary>
        [DataMember]
        public int MaxLenght { get; set; }

        /// <summary>
        /// Regex pattern for this entry
        /// </summary>
        [DataMember]
        public string Regex { get; set; }

        /// <summary>
        /// Indicated if the field is required and should not be empty
        /// </summary>
        [DataMember]
        public bool IsRequired { get; set; }

        /// <see cref="ICloneable"/>
        public object Clone()
        {
            return Clone(false);
        }

        /// <summary>
        /// Method to create a deep or shallow copy of this object
        /// </summary>
        public EntryValidation Clone(bool deep)
        {
            // All value types can be simply copied
            var copy = new EntryValidation
            {
                MinLenght = MinLenght, 
                MaxLenght = MaxLenght, 
                Regex = Regex, 
                IsRequired = IsRequired
            };
            return copy;
        }
    }
}
