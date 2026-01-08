// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.AbstractionLayer.Recipes;

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
    public ProductType Product { get; set; }

    /// <inheritdoc />
    public virtual ProductType Target => Product;

    /// <inheritdoc />
    public override IRecipe Clone()
    {
        return new ProductRecipe(this);
    }
}