using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract]
    internal class ProductCustomization
    {
        [DataMember]
        public ProductTypeModel[] ProductTypes { get; set; }

        [DataMember]
        public RecipeTypeModel[] RecipeTypes { get; set; }

        [DataMember]
        public ProductImporter[] Importers { get; set; }
    }
}