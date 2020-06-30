// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.AbstractionLayer.Products;

namespace Marvin.AbstractionLayer.Recipes
{
    /// <summary>
    /// Recipe to create instances of a product
    /// </summary>
    public class ProductRecipe : Recipe, IProductRecipe
    {
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

        /// <inheritdoc />
        public IProductType Product { get; set; }

        /// <inheritdoc />
        public virtual IProductType Target => Product;

        /// <inheritdoc />
        public override IRecipe Clone()
        {
            return new ProductRecipe(this);
        }
    }
}
