// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;

namespace Marvin.Tools.Wcf.FileSystem
{
    public enum EFileType
    {
        File,
        Directory
    }

    [DataContract]
    public class FileModel
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public EFileType Type { get; set; }

        [DataMember]
        public long FileSize { get; set; }

        [DataMember]
        public DateTime? CreationDate { get; set; }

        [DataMember]
        public string MimeType { get; set; }
    }
}
