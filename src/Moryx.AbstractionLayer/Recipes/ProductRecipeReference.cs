// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.AbstractionLayer.Recipes;

/// <summary>
/// Temporary product recipe which should be replaced by the product management
/// </summary>
public class ProductRecipeReference : RecipeReference, IProductRecipe
{
    /// <inheritdoc />
    public ProductType Product { get; set; }

    /// <inheritdoc />
    public ProductType Target => null;

    /// <summary>
    /// Creates a new instance of the <see cref="ProductRecipeReference"/>
    /// </summary>
    public ProductRecipeReference(long recipeId) : base(recipeId)
    {
    }
}