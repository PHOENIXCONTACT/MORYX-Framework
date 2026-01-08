// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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
    public class SetupRecipe : WorkplanRecipe
    {
        /// <summary>
        /// Recipe of the next job prepared by this
        /// </summary>
        public IProductRecipe TargetRecipe { get; set; }

        /// <summary>
        /// Flag if this setup is a cleanup
        /// </summary>
        public SetupExecution Execution { get; set; }

        /// <summary>
        /// Flags of setup classifications
        /// </summary>
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
