// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    [DataContract]
    public class ProductImporter
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Entry Parameters { get; set; }
    }
}
