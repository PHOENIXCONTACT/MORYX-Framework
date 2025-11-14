// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.VisualInstructions;

namespace Moryx.ControlSystem.Assemble
{
    /// <summary>
    /// Parameters for the <see cref="AssembleActivity"/>
    /// </summary>
    public class AssembleParameters : VisualInstructionParameters
    {
        /// <summary>
        /// Product that may provide additional Parameters
        /// </summary>
        public ProductType Product { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="AssembleParameters"/>
        /// </summary>
        public AssembleParameters() : this([])
        {
        }

        /// <summary>
        /// Create parameters with visual instructions
        /// </summary>
        public AssembleParameters(VisualInstruction[] instructions)
        {
            Instructions = instructions;
        }

        /// <inheritdoc />
        protected override void Populate(IProcess process, Parameters instance)
        {
            base.Populate(process, instance);
            var parameters = (AssembleParameters)instance;

            // Assign product
            var recipe = (IProductRecipe)process.Recipe;
            parameters.Product = recipe.Target;
        }
    }
}
