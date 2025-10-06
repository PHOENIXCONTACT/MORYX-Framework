// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Configuration
{
    /// <summary>
    /// Determines if a config parameter is a local file field
    /// </summary>
    public enum FileSystemPathType
    {
        /// <summary>
        /// File
        /// </summary>
        File = 1,
        /// <summary>
        /// Directory
        /// </summary>
        Directory = 2
    }

    /// <summary>
    /// Determines if a config parameter is a file or directory field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class FileSystemPathAttribute : Attribute
    {
        /// <summary>
        /// Default constructor. Sets <see cref="Type" /> to <see cref="FileSystemPathType.File"/>
        /// </summary>
        public FileSystemPathAttribute() : this(FileSystemPathType.File)
        {
        }

        /// <summary>
        /// Extended constructor
        /// </summary>
        /// <param name="type"></param>
        public FileSystemPathAttribute(FileSystemPathType type)
        {
            Type = type;
        }

        /// <summary>
        /// Returns the selected path type
        /// </summary>
        public FileSystemPathType Type { get; }
    }
}
