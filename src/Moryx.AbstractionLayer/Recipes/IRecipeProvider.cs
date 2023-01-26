// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Recipes
{
    /// <summary>
    /// Provides recipes for production
    /// </summary>
    public interface IRecipeProvider
    {
        /// <summary>
        /// Identity of this provider
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Load recipe by its database id
        /// </summary>
        IRecipe LoadRecipe(long id);

        /// <summary>
        /// A recipe was changed, give users the chance to update their reference
        /// </summary>
        event EventHandler<IRecipe> RecipeChanged;
    }
}
