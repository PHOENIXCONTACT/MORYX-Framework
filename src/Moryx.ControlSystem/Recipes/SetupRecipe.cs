// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.Recipes
{
    /// <summary>
    /// Recipe to instantiate a setup recipe.
    /// </summary>
    [DebuggerDisplay(nameof(SetupRecipe) + " <Id: {" + nameof(Id) + "}, TargetRecipe: {" + nameof(TargetRecipe) + "}, Execution: {" + nameof(Execution) + "}>")]
    public class SetupRecipe : WorkplanRecipe, ISetupRecipe
    {
        /// <inheritdoc />
        public IProductRecipe TargetRecipe { get; set; }

        /// <inheritdoc />
        public SetupExecution Execution { get; set; }

        /// <inheritdoc />
        public SetupClassification SetupClassification { get; set; }

        /// <summary>
        /// Can not clone setup recipes
        /// </summary>
        public override IRecipe Clone()
        {
            throw new NotImplementedException();
        }
    }
}
