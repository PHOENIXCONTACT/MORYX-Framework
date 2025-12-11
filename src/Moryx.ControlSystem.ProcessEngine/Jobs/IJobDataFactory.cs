// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    internal interface IJobDataFactory
    {
        T Create<T>(IWorkplanRecipe recipe, int amount)
            where T : IJobData;

        IProductionJobData Create(JobTemplate template);

        IJobData Create(IWorkplanRecipe recipe, int amount);

        IJobData Restore(JobEntity entity, IWorkplanRecipe recipe);

        void Destroy(IJobData jobData);
    }

    [Component(LifeCycle.Singleton, typeof(IJobDataFactory))]
    internal class JobDataFactory : IJobDataFactory
    {
        public IContainerJobDataFactory InternalFactory { get; set; }

        public T Create<T>(IWorkplanRecipe recipe, int amount) where T : IJobData
        {
            return (T)Create(recipe, amount);
        }

        public IProductionJobData Create(JobTemplate template)
        {
            return Create<IProductionJobData>((ProductionRecipe)template.Recipe, (int)template.Amount);
        }

        public IJobData Create(IWorkplanRecipe recipe, int amount)
        {
            IJobData jobData = null;

            if (recipe is ProductionRecipe)
                jobData = InternalFactory.CreateProductionJob(recipe, amount);

            if (recipe is SetupRecipe)
                jobData = InternalFactory.CreateSetupJob(recipe);

            ValidateCreatedJob(jobData, recipe);

            return jobData;
        }

        public IJobData Restore(JobEntity entity, IWorkplanRecipe recipe)
        {
            IJobData jobData = null;

            if (recipe is ProductionRecipe)
                jobData = InternalFactory.CreateProductionJob(recipe, entity);

            if (recipe is SetupRecipe)
                jobData = InternalFactory.CreateSetupJob(recipe, entity);

            ValidateCreatedJob(jobData, recipe);

            return jobData;
        }

        private static void ValidateCreatedJob(IJobData jobData, IWorkplanRecipe recipe)
        {
            if (jobData == null)
                throw new InvalidOperationException("Cannot find job type for recipe of type: " + recipe.GetType().FullName);
        }

        public void Destroy(IJobData jobData) => InternalFactory.Destroy(jobData);
    }

    [PluginFactory]
    internal interface IContainerJobDataFactory
    {
        IProductionJobData CreateProductionJob(IWorkplanRecipe recipe, int amount);

        IProductionJobData CreateProductionJob(IWorkplanRecipe recipe, JobEntity entity);

        ISetupJobData CreateSetupJob(IWorkplanRecipe recipe);

        ISetupJobData CreateSetupJob(IWorkplanRecipe recipe, JobEntity entity);

        void Destroy(IJobData jobData);
    }
}
