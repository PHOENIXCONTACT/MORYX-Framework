// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints.Models
{
    [DataContract]
    public class ProductCustomization
    {
        [DataMember]
        public ProductDefinitionModel[] ProductTypes { get; set; }

        [DataMember]
        public RecipeDefinitionModel[] RecipeTypes { get; set; }

        [DataMember]
        public ProductImporter[] Importers { get; set; }
    }
}
