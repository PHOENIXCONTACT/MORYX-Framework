// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Products;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    [DataContract]
    public class ProductModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ProductState State { get; set; }

        [DataMember]
        public string Identifier { get; set; }

        [DataMember]
        public short Revision { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public Entry Properties { get; set; }

        // TODO: AL6 Remove Files and rename FileModels to Files
        [DataMember]
        [Obsolete("Use FileModels Instead")]
        public ProductFile[] Files { get; set; }

        [DataMember]
        public ProductFileModel[] FileModels { get; set; }

        [DataMember]
        public PartConnector[] Parts { get; set; }

        [DataMember]
        public RecipeModel[] Recipes { get; set; }
    }
}
