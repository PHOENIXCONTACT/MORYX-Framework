// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Serialization
{
    [AttributeUsage(AttributeTargets.All)]
    public class EntrySerializeAttribute : Attribute
    {
        /// <summary>
        /// Configured mode for the entry to serialize
        /// </summary>
        public EntrySerializeMode Mode { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="EntrySerializeAttribute"/>. The default mode is <see cref="EntrySerializeMode.Always"/>
        /// </summary>
        public EntrySerializeAttribute() : this(EntrySerializeMode.Always)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EntrySerializeAttribute"/>.
        /// </summary>
        public EntrySerializeAttribute(EntrySerializeMode mode)
        {
            Mode = mode;
        }
    }

    /// <summary>
    /// Mode to for the <see cref="EntrySerializeAttribute"/>
    /// </summary>
    public enum EntrySerializeMode
    {
        Always,
        Never
    }
}

