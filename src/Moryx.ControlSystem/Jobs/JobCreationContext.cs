// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#nullable enable

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
        /// Context for a <see cref="Job"/> based on a <see cref="JobTemplate"/> using the given <paramref name="recipe"/> and <paramref name="amount"/>
        /// </summary>
        public JobCreationContext(ProductionRecipe recipe, uint amount)
        {
            _templates.Add(new JobTemplate(recipe, amount));
        }

        private readonly List<JobTemplate> _templates = [];
        /// <summary>
        /// Recipe for the job to produce
        /// </summary>
        public IReadOnlyList<JobTemplate> Templates => _templates;

        /// <summary>
        /// Position of the new job
        /// </summary>
        public JobPosition Position { get; private set; }

        /// <summary>
        /// Add another <see cref="JobTemplate"/> to this <see cref="JobCreationContext"/> based on the
        /// given <paramref name="recipe"/> and <paramref name="amount"/>
        /// </summary>
        public JobCreationContext Add(ProductionRecipe recipe, uint amount)
        {
            _templates.Add(new JobTemplate(recipe, amount));
            return this;
        }

        /// <summary>
        /// Add another <see cref="PreallocatedJobTemplate"/> to this <see cref="JobCreationContext"/>
        /// based on the given <paramref name="recipe"/> and <paramref name="amount"/>
        /// </summary>
        /// <returns>The <see cref="AllocationToken"/> to monitor and modify the preallocation</returns>
        public AllocationToken Preallocate(ProductionRecipe recipe, uint amount)
        {
            var template = new PreallocatedJobTemplate(recipe, amount);
            _templates.Add(template);
            return template.AllocationToken;
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
    /// Single job template for a <see cref="Job"/>
    /// </summary>
    [DebuggerDisplay(nameof(JobTemplate) + ": <Recipe: {" + nameof(Recipe) + "}, Amount: {" + nameof(Amount) + "}>")]
    public record JobTemplate
    {
        /// <summary>
        /// Create a new job template from recipe and amount
        /// </summary>
        internal JobTemplate(ProductionRecipe recipe, uint amount)
        {
            Recipe = recipe;
            Amount = amount;
        }

        /// <summary>
        /// Recipe of the new job
        /// </summary>
        public ProductionRecipe Recipe { get; }

        /// <summary>
        /// Amount of the new job
        /// </summary>
        public uint Amount { get; }
    }

    /// <summary>
    /// Single job template representing a preallocation for a <see cref="Job"/>, which can be monitored via the <see cref="AllocationToken"/>.
    /// </summary>
    [DebuggerDisplay(nameof(PreallocatedJobTemplate) + ": <Recipe: {" + nameof(Recipe) + "}, Amount: {" + nameof(Amount) + "}, Allocation Status: {" + nameof(AllocationToken.Status) + "}>")]
    public record PreallocatedJobTemplate : JobTemplate
    {
        /// <summary>
        /// Create a new job template from recipe and amount
        /// </summary>
        internal PreallocatedJobTemplate(ProductionRecipe recipe, uint amount) : base(recipe, amount)
        {
            AllocationToken = new AllocationToken(amount);
        }

        /// <summary>
        /// Gets the allocation token for this preallocated job
        /// </summary>
        public AllocationToken AllocationToken { get; }
    }
}
