// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;

namespace Moryx.Serialization
{
    /// <summary>
    /// All validation rules of an entry
    /// </summary>
    [DataContract]
    public class EntryValidation : ICloneable
    {
        /// <summary>
        /// Minimum value or minimal length of strings
        /// </summary>
        [DataMember]
        public double Minimum { get; set; }

        /// <summary>
        /// Maximum value or maximal length of strings
        /// </summary>
        [DataMember]
        public double Maximum { get; set; }

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

        /// <summary>
        /// Creates a new <see cref="EntryValidation"/> instance initializing <see cref="EntryValidation.Maximum"/> 
        /// and <see cref="EntryValidation.Minimum"/> validation to the largest possible range.
        /// </summary>
        public EntryValidation()
        {
            Minimum = double.MinValue;
            Maximum = double.MaxValue;
        }

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
                Minimum = Minimum, 
                Maximum = Maximum, 
                Regex = Regex, 
                IsRequired = IsRequired
            };
            return copy;
        }
    }
}
