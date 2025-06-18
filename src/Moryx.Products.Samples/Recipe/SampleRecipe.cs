using Moryx.AbstractionLayer.Recipes;
using Moryx.Serialization;
using System.ComponentModel;

namespace Moryx.Products.Samples.Recipe
{
    public class SampleRecipe: ProductRecipe
    {
        [EntrySerialize]
        [DisplayName("Color Code")]
        [Description("Numeric code of the color")]
        public int CaseColorCode { get; set; }

        [EntrySerialize]
        [DisplayName("Way of production")]
        public ProductionType ProductionType { get; set; }

        [EntrySerialize]
        [DisplayName("Name of the skilled worker")]
        public string NameSkilledWorker { get; set; }

        [EntrySerialize]
        [DisplayName("Production time")]
        public double ProductionTime { get; set; }
    }

    public enum ProductionType
    {
        manually,
        automatically,
        withoutAir
    }
}
