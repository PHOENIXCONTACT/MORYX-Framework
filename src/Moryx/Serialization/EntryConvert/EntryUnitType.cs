// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization
{
    /// <summary>
    /// Enum describes the unit type of a entry
    /// </summary>
    public enum EntryUnitType
    {
        /// <summary>
        /// No unit
        /// </summary>
        None,
        /// <summary>
        /// Unit password
        /// </summary>
        Password,
        /// <summary>
        /// Unit file
        /// </summary>
        File,
        /// <summary>
        /// Unit directory
        /// </summary>
        Directory,
        /// <summary>
        /// Unit with <see cref="System.FlagsAttribute"/>
        /// </summary>
        Flags,
    }
}
