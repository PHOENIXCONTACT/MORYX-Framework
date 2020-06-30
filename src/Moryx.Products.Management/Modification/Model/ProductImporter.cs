// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Products.Management.Importers;
using Moryx.Serialization;

namespace Moryx.Products.Management.Modification
{
    [DataContract]
    internal class ProductImporter
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Entry Parameters { get; set; }
    }
}
