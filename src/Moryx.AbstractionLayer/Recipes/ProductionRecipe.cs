// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;

namespace Moryx.AbstractionLayer.Recipes;

/// <summary>
/// Recipe which contains a workplan and product
/// </summary>
public class ProductionRecipe : WorkplanRecipe, IProductRecipe
{
    /// <inheritdoc />
    public ProductType Product { get; set; }

    /// <inheritdoc />
    public virtual ProductType Target => Product;

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
    public override Process CreateProcess() =>
        new ProductionProcess { Recipe = this };

    /// <inheritdoc />
    public override IRecipe Clone() =>
        new ProductionRecipe(this);
}