// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.Recipes
{
    /// <summary>
    /// Indicates if the recipe is used for setup cases
    /// </summary>
    public interface ISetupRecipe : IWorkplanRecipe
    {
        /// <summary>
        /// Recipe of the next job prepared by this
        /// </summary>
        IProductRecipe TargetRecipe { get; set; }

        /// <summary>
        /// Flag if this setup is a cleanup
        /// </summary>
        SetupExecution Execution { get; }

        /// <summary>
        /// Flags of setup classifications
        /// </summary>
        SetupClassification SetupClassification { get; }
    }
}
