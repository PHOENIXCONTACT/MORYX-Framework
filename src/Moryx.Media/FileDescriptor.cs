// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media
{
    /// <summary>
    /// Base class to hold information describing a media file
    /// </summary>
    [DataContract]
    public abstract class FileDescriptor
    {
        /// <inheritdoc />
        [DataMember]
        public string FileHash { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Extension { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string MimeType { get; set; }

        /// <inheritdoc />
        [DataMember]
        public long Size { get; set; }
    }
}
