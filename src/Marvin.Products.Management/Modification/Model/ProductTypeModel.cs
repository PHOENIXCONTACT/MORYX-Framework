// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract]
    internal class ProductTypeModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string BaseType { get; set; }
    }
}
