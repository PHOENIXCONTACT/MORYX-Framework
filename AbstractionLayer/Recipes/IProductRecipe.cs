namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Recepie which is related to a product
    /// </summary>
    public interface IProductRecipe : IWorkplanRecipe
    {
        /// <summary>
        /// The product that defines this recipe and is the final
        /// product.
        /// </summary>
        IProduct Product { get; set; }

        /// <summary>
        /// The target product that should be produced with this recipe.
        /// In most cases this equals <see cref="Product"/>, but it does
        /// not have to.
        /// </summary>
        IProduct Target { get; }
    }
}