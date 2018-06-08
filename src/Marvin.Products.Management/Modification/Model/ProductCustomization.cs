using System.Runtime.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract]
    internal class ProductCustomization
    {
        [DataMember]
        public bool ReleasedProductsEditable { get; set; }

        [DataMember]
        public bool HasRecipes { get; set; }

        [DataMember]
        public string[] ProductTypes { get; set; }

        [DataMember]
        public string[] RecipeTypes { get; set; }

        [DataMember]
        public StorageValue[] StorageValues { get; set; }

        [DataMember]
        public ProductImporter[] Importers { get; set; }
    }
}