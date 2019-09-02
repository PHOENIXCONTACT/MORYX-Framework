namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Recipe to instantiate a maintenance recipe.
    /// </summary>
    public abstract class MaintenanceRecipe : Recipe
    {
        /// <inheritdoc />
        public sealed override string Type => nameof(MaintenanceRecipe);

        /// <summary>
        /// Default constructor to create a new recipe
        /// </summary>
        protected MaintenanceRecipe()
        {

        }

        /// <summary>
        /// Clone constructor to create a new maintenance recipe
        /// </summary>
        protected MaintenanceRecipe(MaintenanceRecipe source)
            : base(source)
        {

        }
    }
}
