// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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
        public DispatchContext(ProductionRecipe recipe, uint amount)
        {
            Recipe = recipe;
            Amount = amount;
        }

        /// <summary>
        /// Recipe which should be used for the dispatching
        /// </summary>
        public ProductionRecipe Recipe { get; }

        /// <summary>
        /// The amount which should be dispatched
        /// </summary>
        public uint Amount { get; }
    }
}