// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;

namespace Moryx.Orders
{
    /// <summary>
    /// Context to encapsulate what should be dispatched
    /// </summary>
    public class DispatchContext
    {
        /// <summary>
        /// Default constructor with the needed information for the dispatching
        /// </summary>
        public DispatchContext(IProductionRecipe recipe, uint amount)
        {
            Recipe = recipe;
            Amount = amount;
        }

        /// <summary>
        /// Recipe which should be used for the dispatching
        /// </summary>
        public IProductionRecipe Recipe { get; }

        /// <summary>
        /// The amount which should be dispatched
        /// </summary>
        public uint Amount { get; }
    }
}