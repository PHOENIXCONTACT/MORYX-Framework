// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Properties;

namespace Moryx.AbstractionLayer.Recipes
{
    /// <summary>
    /// Exception thrown when a recipe could not be found by its id
    /// </summary>
    public class RecipeNotFoundException : Exception
    {
        /// <summary>
        /// Initialize exception for non existing id
        /// </summary>
        /// <param name="id">Id that was not found in database</param>
        public RecipeNotFoundException(long id)
            : base(string.Format(Strings.RecipeNotFoundException_Message, id))
        {
        }
    }
}
