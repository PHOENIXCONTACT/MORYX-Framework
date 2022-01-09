// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Products.Management.Modification
{
    [DataContract]
    internal class ProductFileModel
    {
        [DataMember]
        public string PropertyName { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string MimeType { get; set; }

        [DataMember]
        public string FilePath { get; set; }

        [DataMember]
        public string FileHash { get; set; }
    }
}
