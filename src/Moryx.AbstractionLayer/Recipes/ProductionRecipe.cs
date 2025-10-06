// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.AbstractionLayer.Recipes
{
    /// <summary>
    /// Recipe which contains a workplan and product
    /// </summary>
    public class ProductionRecipe : WorkplanRecipe, IProductionRecipe
    {
        /// <inheritdoc />
        public IProductType Product { get; set; }

        /// <inheritdoc />
        public virtual IProductType Target => Product;

        /// <summary>
        /// Prepare recipe by filling DisabledSteps and TaskAssignment properties
        /// </summary>
        public ProductionRecipe()
        {
        }

        /// <summary>
        /// Clone a production recipe
        /// </summary>
        protected ProductionRecipe(ProductionRecipe source)
            : base(source)
        {
            Product = source.Product;
        }

        /// <summary>
        /// Create a <see cref="ProductionProcess"/> for this recipe
        /// </summary>
        public override IProcess CreateProcess() =>
            new ProductionProcess { Recipe = this };

        /// <inheritdoc />
        public override IRecipe Clone() =>
            new ProductionRecipe(this);
    }
}