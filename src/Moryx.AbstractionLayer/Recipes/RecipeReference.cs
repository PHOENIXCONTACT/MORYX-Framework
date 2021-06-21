// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Recipes
{
    /// <summary>
    /// Temporary recipe which should be replaced by the product management
    /// </summary>
    public class RecipeReference : IRecipe
    {
        /// <inheritdoc />
        public long Id { get; set; }

        /// <inheritdoc />
        public long TemplateId { get; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public int Revision { get; set; }

        /// <inheritdoc />
        public RecipeState State { get; set; }

        /// <inheritdoc />
        public RecipeClassification Classification { get; set; }

        /// <inheritdoc />
        public IRecipeProvider Origin { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="RecipeReference"/>
        /// </summary>
        public RecipeReference(long recipeId)
        {
            Id = recipeId;
        }

        /// <inheritdoc />
        public IProcess CreateProcess()
        {
            throw new InvalidOperationException($"{nameof(RecipeReference)} cannot create processes.");
        }

        /// <inheritdoc />
        public IRecipe Clone()
        {
            throw new InvalidOperationException($"{nameof(RecipeReference)} cannot be cloned!");
        }
    }
}
