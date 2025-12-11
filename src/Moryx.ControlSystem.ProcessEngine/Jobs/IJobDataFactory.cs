// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Production;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    internal interface IJobDataFactory
    {
        T Create<T>(IWorkplanRecipe recipe, int amount)
            where T : IJobData;

        IJobData Create(JobTemplate template, int adjustedAmount);

        IJobData Restore(JobEntity entity, IWorkplanRecipe recipe);

        void Destroy(IJobData jobData);
    }

    [Component(LifeCycle.Singleton, typeof(IJobDataFactory))]
    internal class JobDataFactory : IJobDataFactory
    {
        public IContainerJobDataFactory InternalFactory { get; set; }

        public T Create<T>(IWorkplanRecipe recipe, int amount) where T : IJobData
        {
            return recipe switch
            {
                ProductionRecipe => (T)InternalFactory.CreateProductionJob(recipe, amount),
                SetupRecipe => (T)InternalFactory.CreateSetupJob(recipe),
                _ => throw new InvalidOperationException("Cannot find job type for template of type: " + recipe.GetType().FullName),
            };
        }

        public IJobData Create(JobTemplate template, int adjustedAmount)
        {
            return template switch
            {
                PreallocatedJobTemplate preallocated => InternalFactory.CreatePreallocatedJob(preallocated.Recipe, adjustedAmount, preallocated.AllocationToken),
                _ => InternalFactory.CreateProductionJob(template.Recipe, adjustedAmount),
            };
        }

        public IJobData Restore(JobEntity entity, IWorkplanRecipe recipe)
        {
            return recipe switch
            {
                ProductionRecipe => InternalFactory.CreateProductionJob(recipe, entity),
                SetupRecipe => InternalFactory.CreateSetupJob(recipe, entity),
                _ => throw new InvalidOperationException("Cannot find job type for template of type: " + recipe.GetType().FullName),
            };
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

        IPreallocatedJobData CreatePreallocatedJob(IWorkplanRecipe recipe, int amount, AllocationToken allocationToken);

        void Destroy(IJobData jobData);
    }
}
