// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media.Server.Endpoint
{
    [DataContract]
    [KnownType(typeof(MemoryStream))]
    [KnownType(typeof(FileStream))]
    internal class PreviewUploadRequest
    {
        [DataMember]
        public Guid ContentId { get; set; }

        [DataMember]
        public string VariantName { get; set; }

        [DataMember]
        public Stream ContentStream { get; set; }
    }
}

