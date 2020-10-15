using System.Runtime.Serialization;

namespace Moryx.Products.Management.Modification
{
    [DataContract]
    internal class SaveRecipeRequest
    {
        [DataMember]
        public RecipeModel Recipe { get; set; }
    }
}
