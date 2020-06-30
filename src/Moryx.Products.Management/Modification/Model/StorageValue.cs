// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Products.Management.Modification
{
    [DataContract]
    internal class StorageValue
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string[] Values { get; set; }
    }
}
