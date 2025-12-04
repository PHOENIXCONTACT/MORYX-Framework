// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// Context to create a job on the <see cref="IJobManagement"/> facade
    /// </summary>
    public class JobCreationContext
    {
        /// <summary>
        /// Empty constructor for cases like iterations while creating
        /// </summary>
        public JobCreationContext()
        {
        }

        /// <summary>
        /// Context for an default job
        /// </summary>
        public JobCreationContext(IProductRecipe recipe, uint amount)
        {
            _templates.Add(new JobTemplate(recipe, amount));
        }

        /// <summary>
        /// Context for an default job
        /// </summary>
        public JobCreationContext(IProductRecipe recipe) : this(recipe, 0)
        {
        }

        private readonly List<JobTemplate> _templates = new();
        /// <summary>
        /// Recipe for the job to produce
        /// </summary>
        public IReadOnlyList<JobTemplate> Templates => _templates;

        /// <summary>
        /// Position of the new job
        /// </summary>
        public JobPosition Position { get; private set; }

        /// <summary>
        /// Add another job to the context
        /// </summary>
        public JobCreationContext Add(ProductionRecipe recipe)
        {
            return Add(recipe, 0);
        }

        /// <summary>
        /// Add another job to the context
        /// </summary>
        public JobCreationContext Add(ProductionRecipe recipe, uint amount)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe), "Recipe should not be null");

            _templates.Add(new JobTemplate(recipe, amount));

            return this;
        }

        /// <summary>
        /// Add this job to the end of the list. This is the default behavior
        /// </summary>
        public JobCreationContext Append()
        {
            Position = new JobPosition(JobPositionType.Append, null);
            return this;
        }

        /// <summary>
        /// Insert this job at position 0
        /// </summary>
        /// <returns></returns>
        public JobCreationContext Topmost()
        {
            Position = new JobPosition(JobPositionType.Start, null);
            return this;
        }

        /// <summary>
        /// Add job before another one
        /// </summary>
        public JobCreationContext Before(Job other)
        {
            Position = new JobPosition(JobPositionType.BeforeOther, other.Id);
            return this;
        }

        /// <summary>
        /// Add job after another
        /// </summary>
        public JobCreationContext After(Job other)
        {
            Position = new JobPosition(JobPositionType.AfterOther, other.Id);
            return this;
        }
    }

    /// <summary>
    /// Single job template that shall be created by the job creation context
    /// </summary>
    [DebuggerDisplay("JobTemplate: <Recipe: {" + nameof(Recipe) + "}, Amount: {" + nameof(Amount) + "}>")]
    public struct JobTemplate
    {
        /// <summary>
        /// Create a new job template from recipe and amount
        /// </summary>
        internal JobTemplate(IProductRecipe recipe, uint amount)
        {
            Recipe = recipe;
            Amount = amount;
        }

        /// <summary>
        /// Recipe of the new job
        /// </summary>
        public IProductRecipe Recipe { get; }

        /// <summary>
        /// Amount of the new job
        /// </summary>
        public uint Amount { get; }
    }
}
