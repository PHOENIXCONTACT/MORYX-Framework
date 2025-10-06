// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media.Server.Endpoint
{
    [DataContract]
    internal class ContentUploadRequest
    {
        [DataMember]
        public Guid? ContentId { get; set; }

        [DataMember]
        public string VariantName { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string File { get; set; }
    }
}

