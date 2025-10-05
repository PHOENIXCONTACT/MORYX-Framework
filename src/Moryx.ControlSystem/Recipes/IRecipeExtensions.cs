// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using System;

namespace Moryx.ControlSystem.Recipes
{
    /// <summary>
    /// Extensions on <see cref="IOrderBasedRecipe"/>s
    /// </summary>
    public static class IRecipeExtensions
    {
        /// <summary>
        /// Gets a string concatinating order and operation number from an <see cref="IOrderBasedRecipe"/> 
        /// or and empty string if none could be found.
        /// </summary>
        /// <param name="recipe">An <see cref="IRecipe"/> that implements <see cref="IOrderBasedRecipe"/> 
        /// or an <see cref="ISetupRecipe"/> targeting an <see cref="IOrderBasedRecipe"/></param>
        /// <param name="seperator">Separation char between order number and operation number</param>
        public static string GetOrderOperationString(this IRecipe recipe, string seperator = "-")
        {
            var target = (recipe is ISetupRecipe setup ? setup.TargetRecipe : recipe) as IOrderBasedRecipe;
            return $"{target?.OrderNumber}{(target is null ? "" : seperator)}{target?.OperationNumber}";            
        }
    }
}
