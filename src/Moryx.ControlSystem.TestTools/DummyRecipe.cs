// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.TestTools;
using Moryx.ControlSystem.TestTools.Tasks;
using Moryx.Workplans;

namespace Moryx.ControlSystem.TestTools
{
    /// <summary>
    /// Implementation of a recipe for testing purposes
    /// </summary>
    public class DummyRecipe : ProductionRecipe
    {
        /// <summary>
        /// Creates a new instances of the <see cref="DummyResult"/>
        /// </summary>
        public DummyRecipe()
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="DummyResult"/> forwarding the constructor to the base class
        /// </summary>
        protected DummyRecipe(DummyRecipe source) : base(source)
        {
        }


        /// <summary>
        /// Creates a <see cref="ProductionRecipe"/> and fills the properties with reasonable values
        /// </summary>
        /// <param name="id">The id of the created recipe</param>
        /// <returns>The created recipe</returns>
        public static ProductionRecipe BuildRecipe(long id = 1)
        {
            var workplan = BuildWorkplan();

            var recipe = new ProductionRecipe
            {
                Id = id,
                Name = workplan.Name,
                Revision = 1,
                State = RecipeState.Released,
                Workplan = workplan,
                Product = new DummyProductType()
            };

            return recipe;
        }

        /// <summary>
        ///  Creates a <see cref="Workplan"/> and fills the properties with reasonable values
        /// </summary>
        /// <returns>The created workplan</returns>
        public static Workplan BuildWorkplan()
        {
            var workplan = new Workplan
            {
                Name = "DummyWorkplan"
            };

            // Workplan boundaries
            var start = WorkplanInstance.CreateConnector("StartNode", NodeClassification.Start);
            var mounted = WorkplanInstance.CreateConnector("MountedNode", NodeClassification.Intermediate);
            var done = WorkplanInstance.CreateConnector("DoneNode", NodeClassification.Intermediate);
            var end = WorkplanInstance.CreateConnector("EndNode", NodeClassification.End);
            var failed = WorkplanInstance.CreateConnector("Failed", NodeClassification.Failed);

            workplan.Add(start, mounted, done, end, failed);

            // Mount task
            var mount = BuildTaskStep(new MountTask(), start, mounted);

            // The really important task
            var task = BuildTaskStep(new AssignIdentityTask(), mounted, done, failed);

            // Unmount of successful article
            var unmount = BuildTaskStep(new UnmountTask(), done, end);

            workplan.Add(mount, task, unmount);

            return workplan;
        }

        private static IWorkplanStep BuildTaskStep(IWorkplanStep task, IConnector input, params IConnector[] outputs)
        {
            task.Inputs[0] = input;
            for (var i = 0; i < task.Outputs.Length; i++)
            {
                if (outputs.Length > i)
                    task.Outputs[i] = outputs[i];
                else
                    task.Outputs[i] = outputs[outputs.Length - 1]; // Use last for all missing
            }
            return task;
        }

        /// <summary>
        /// Creates a new <see cref="DummyRecipe"/> that is a clone of this one
        /// </summary>
        /// <returns>A clone of this recipe</returns>
        public override IRecipe Clone()
        {
            return new DummyRecipe(this);
        }
    }
}
