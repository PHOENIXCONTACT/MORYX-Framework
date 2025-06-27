// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Runtime.Serialization;

namespace Moryx.Orders.Documents
{
    /// <summary>
    /// Document which the file source is the file system
    /// </summary>
    [DataContract]
    public class FileSystemDocument : Document
    {
        /// <summary>
        /// Path to the file at the file system
        /// </summary>
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FileSystemDocument()
        {
        }

        /// <summary>
        /// Constructor to create a file system document
        /// </summary>
        public FileSystemDocument(string number, short revision, string path) : base(number, revision)
        {
            Path = path;
        }

        /// <inheritdoc />
        public override Stream GetStream()
        {
            return new FileStream(Path, FileMode.Open);
        }
    }
}