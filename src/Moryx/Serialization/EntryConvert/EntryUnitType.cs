// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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
        FilePath,
        /// <summary>
        /// Unit directory
        /// </summary>
        DirectoryPath,
        /// <summary>
        /// Unit with <see cref="System.FlagsAttribute"/>
        /// </summary>
        Flags,
        /// <summary>
        /// Unit is a base64 string
        /// </summary>
        Base64
    }
}
