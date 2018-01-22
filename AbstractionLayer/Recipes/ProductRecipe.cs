namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Recipe to create instances of a product
    /// </summary>
    public class ProductRecipe : WorkplanRecipe, IProductRecipe
    {
        /// 
        public override string Type => nameof(ProductRecipe);

        /// <summary>
        /// Create a new product recipe
        /// </summary>
        public ProductRecipe()
        {
        }

        /// <summary>
        /// Clone a product recipe 
        /// </summary>
        protected ProductRecipe(ProductRecipe source)
            : base(source)
        {
            Product = source.Product;
        }

        /// 
        public IProduct Product { get; set; }
        
        /// <inheritdoc />
        public virtual IProduct Target => Product;

        /// <summary>
        /// Create a <see cref="ProductionProcess"/> for this recipe
        /// </summary>
        public override IProcess CreateProcess()
        {
            return new ProductionProcess { Recipe = this };
        }

        /// <inheritdoc />
        public override IRecipe Clone()
        {
            return new ProductRecipe(this);
        }
    }
}
