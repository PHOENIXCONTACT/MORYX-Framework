// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// A recipe is used to provide data for a process and to provide additional parameters.
    /// </summary>
    public interface IRecipe : IPersistentObject
    {
        /// <summary>
        /// Name of the recipe
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Revision of this recipe
        /// </summary>
        int Revision { get; set; }

        /// <summary>
        /// This recipe's state
        /// </summary>
        RecipeState State { get; set; }

        /// <summary>
        /// Classification of the recipe
        /// </summary>
        RecipeClassification Classification { get; set; }

        /// <summary>
        /// Provider that created this recipe
        /// </summary>
        IRecipeProvider Origin { get; set; }

        /// <summary>
        /// Create process that can execute this recipe
        /// </summary>
        IProcess CreateProcess();

        /// <summary>
        /// Creates a clone of the current recipe
        /// </summary>
        IRecipe Clone();
    }
}
