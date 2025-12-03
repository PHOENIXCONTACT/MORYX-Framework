// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Workplans;

namespace Moryx.Products.Management;

/// <summary>
/// Extensions for the <see cref="ProductType"/>
/// </summary>
internal static class InternalExtensions
{
    /// <summary>
    /// Returns the string representation of the product type
    /// </summary>
    /// <param name="productType">Source product type</param>
    /// <returns>Type identifier as string</returns>
    public static string ProductTypeName(this ProductType productType)
    {
        return productType.GetType().FullName;
    }

    /// <summary>
    /// Returns the string representation of the recipe type
    /// </summary>
    /// <param name="recipe">Source recipe</param>
    /// <returns>Type identifier as string</returns>
    public static string RecipeTypeName(this IRecipe recipe)
    {
        return recipe.GetType().FullName;
    }

    /// <summary>
    /// Returns the string representation of the workplan step type
    /// </summary>
    /// <param name="workplanStep">Source Workplan-step</param>
    /// <returns>Type identifier as string</returns>
    public static string WorkplanStepTypeName(this IWorkplanStep workplanStep)
    {
        return workplanStep.GetType().FullName;
    }

    /// <summary>
    /// Returns the string representation of the product instance type
    /// </summary>
    /// <param name="productInstance">Source product instance</param>
    /// <returns>Type identifier as string</returns>
    public static string ProductInstanceTypeName(this ProductInstance productInstance)
    {
        return productInstance.GetType().FullName;
    }
}
