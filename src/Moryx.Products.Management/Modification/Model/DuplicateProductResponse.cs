// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Products.Management.Modification
{
    [DataContract]
    internal class DuplicateProductResponse
    {
        [DataMember]
        public bool IdentityConflict { get; set; }

        [DataMember]
        public bool InvalidSource { get; set; }

        [DataMember]
        public ProductModel Duplicate { get; set; }
    }
}
