// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Model
{
    /// <summary>
    /// Used to flag a entity property as unicode string
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IsUnicodeAttribute : Attribute
    {
        /// <summary>
        /// True if the property should be a unicode string
        /// </summary>
        public bool Unicode { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="IsUnicodeAttribute"/>
        /// Flags the property as a unicode string
        /// </summary>
        public IsUnicodeAttribute() : this(true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="IsUnicodeAttribute"/>
        /// </summary>
        public IsUnicodeAttribute(bool isUnicode)
        {
            Unicode = isUnicode;
        }
    }
}
