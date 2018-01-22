namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Recipe to instantiate a maintenacne recipe.
    /// </summary>
    public abstract class MaintenanceRecipe : Recipe
    {
        /// 
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
