// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Marvin.Tools.Wcf.FileSystem
{
    [DataContract]
    public class RemoteFile
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public long FileSize { get; set; }

        [DataMember]
        public string RelativeTargetPath { get; set; }

        [DataMember]
        public string Base64File { get; set; }
    }
}
