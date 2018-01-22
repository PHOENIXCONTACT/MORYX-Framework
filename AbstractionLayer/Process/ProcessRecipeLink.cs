namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Links a process to its recipe.
    /// </summary>
    public class ProcessRecipeLink
    {
        /// <summary>
        /// The recipe provider's name
        /// </summary>
        public string RecipeProvider { get; set; } 

        /// <summary>
        /// The ID of the recipe used to handle this process.
        /// </summary>
        public long RecipeId { get; set; }
    }
}