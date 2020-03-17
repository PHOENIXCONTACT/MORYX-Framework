// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Base class for all recipes
    /// </summary>
    public abstract class Recipe : IRecipe, ICloneable
    {
        /// <summary>
        /// Default constructor to create a new recipe
        /// </summary>
        protected Recipe()
        {
        }

        /// <summary>
        /// Create recipe clone from source
        /// </summary>
        protected Recipe(Recipe source)
        {
            Name = source.Name;
            Revision = source.Revision;
            State = source.State;
            Origin = source.Origin;
            Classification = source.Classification | RecipeClassification.Clone;
        }

        /// <inheritdoc />
        public long Id { get; set; }

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

        /// <inheritdoc />
        public virtual IProcess CreateProcess()
        {
            return new Process { Recipe = this };
        }

        /// <inheritdoc />
        public abstract IRecipe Clone();

        /// <summary>
        /// Implement ICloneable explicit, maybe we need it later
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
