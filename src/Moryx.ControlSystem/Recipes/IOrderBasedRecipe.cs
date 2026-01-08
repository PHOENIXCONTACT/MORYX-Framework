// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.Recipes
{
    /// <summary>
    /// Interface for recipes which based on an order/operation
    /// </summary>
    public interface IOrderBasedRecipe : IRecipe
    {
        /// <summary>
        /// Order number
        /// </summary>
        string OrderNumber { get; set; }

        /// <summary>
        /// Operation number
        /// </summary>
        string OperationNumber { get; set; }
    }
}
