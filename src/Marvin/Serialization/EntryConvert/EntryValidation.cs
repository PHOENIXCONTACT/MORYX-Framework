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
    public partial class EntryValidation : ICloneable
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

        /// <summary>
        /// Erstellt ein neues Objekt, das eine Kopie der aktuellen Instanz darstellt.
        /// </summary>
        /// <returns>
        /// Ein neues Objekt, das eine Kopie dieser Instanz ist.
        /// </returns>
        public object Clone()
        {
            return Clone(false);
        }
    }
}
