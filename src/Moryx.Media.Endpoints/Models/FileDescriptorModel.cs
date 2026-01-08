// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media.Endpoints.Models;

[DataContract]
internal abstract class FileDescriptorModel
{
    /// <summary>
    /// Size in bytes of the original file
    /// </summary>
    [DataMember]
    public long Size { get; set; }

    /// <summary>
    /// Hash of this content
    /// </summary>
    [DataMember]
    public string FileHash { get; set; }

    /// <summary>
    /// Url of the image to receive from web service
    /// </summary>
    [DataMember]
    public string FileUrl { get; set; }

    /// <summary>
    /// Extension of the file
    /// </summary>
    [DataMember]
    public string Extension { get; set; }

    /// <summary>
    /// Mime type of the original file
    /// </summary>
    [DataMember]
    public string MimeType { get; set; }
}